using System;
using LogsAndExceptions;

namespace Budget
{
    public class BankAccount : Wallet
    {
        public string _id;

        public decimal Balance
        {
            get => this._money;
            private set => this._money = value;
        }

        protected delegate void NotEnoughMoney(object sender, WalletHandlerArgs handler);
        protected event NotEnoughMoney EmptyWallet;
        public BankAccount(decimal moneyAtStart, string ID) : base(moneyAtStart)
        {
            this.EmptyWallet = NotEnough; 
            if (moneyAtStart < 0)
            {
                EmptyWallet?.Invoke(this, new WalletHandlerArgs("Your balance can`t be a negative."));
                return;
            }
            this._money = moneyAtStart;
            this._id = ID;
            
        }

        public override void SpendMoney(ActivityOrProduct activity)
        {
            if (this._money < activity.Price)
            {
                Logs.LogException( new ArgumentException("Negative Balance"));
                InfoAboutDeal.LogToFile(this._id, "Not enough money. You tried to spend on " + activity.Name + 
                                                  " about ", activity.Price, this);
                EmptyWallet?.Invoke(this, new WalletHandlerArgs("You can`t spend money, " +
                                                               "your balance is negative"));
                return;
            }

            this._money -= activity.Price;
            InfoAboutDeal.LogToFile(this._id, activity.Name, activity.Price, this);
        }

        public override void GetMoney(decimal sum)
        {
            if (sum > this.Balance)
            {
                Logs.LogException(new ArgumentException("You don`t have enough money."));
                EmptyWallet?.Invoke(this, new WalletHandlerArgs("Недостаточно денег на счету."));
                return;
            }
            if (sum < 0)
            {
                Logs.LogException(new ArgumentException("You can`t get a negative amount of money."));
                return;
            }

            this._money -= sum;
            InfoAboutDeal.LogToFile(this._id, "You withdrew ", sum, this);
        }

        public override void AddMoney(ActivityOrProduct activity)
        {
            if (activity.Price < 0)
            {
                Logs.LogException(new ArgumentException("Price can`t be a negative."));
                Console.Error.WriteLine();
                return;
            }

            this.Balance += activity.Price;
            InfoAboutDeal.LogToFile(this._id, $"Received from \'{activity.Name}\'", activity.Price, this);
            Console.WriteLine($"On \'{this._id}\' card was received {activity.Price} from {activity.Name}");
        }

        // Function that merge two bank accounts.
        public static BankAccount operator +(BankAccount fst, BankAccount scd)
        {
            fst = new BankAccount(fst.Balance + scd.Balance, fst._id);
            var temp = scd.Balance;
            scd.Balance = 0;
            Console.WriteLine($"All money from \'{scd._id}\' were transferred to \'{fst._id}\'");
            InfoAboutDeal.LogToFile(fst._id, $"Received from \'{scd._id}\' card", temp, fst);
            InfoAboutDeal.LogToFile(scd._id, $"Transferred to \'{fst._id}\' card", temp, scd);
            return fst; 
        }

        public static void Transfer(BankAccount from, BankAccount to, decimal sum)
        {
            if (sum > from.Balance)
            {
                
                InfoAboutDeal.LogToFile(from._id, $"You tried to transfer to {to._id} card ", sum, from);
                from.EmptyWallet?.Invoke(from,new WalletHandlerArgs("Not enough money."));
                return;
            }

            to.Balance += sum;
            from.Balance -= sum;
            InfoAboutDeal.LogToFile(to._id, $"Received from {from._id}", sum, to);
            Console.WriteLine($"From \'{from._id}\' about ${sum} was transferred to \'{to._id}\'.");
            InfoAboutDeal.LogToFile(from._id, $"Transferred to {from._id} about", sum, from);
        }
        
        private static void NotEnough(object sender, WalletHandlerArgs handler)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(handler.Message);
            Console.ResetColor();
        }
        
    }
}