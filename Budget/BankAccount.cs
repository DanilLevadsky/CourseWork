using System;
using LogsAndExceptions;

namespace Budget
{
    public class BankAccount : Wallet, IWallet
    {
        public string _id;

        public BankAccount(decimal moneyAtStart, string ID) : base(moneyAtStart)
        {
            InvalidCardOperation = Errors;
            SuccessfulOperation = Success;
            if (moneyAtStart < 0)
            {
                InvalidCardOperation?.Invoke(this, new WalletHandlerArgs("Your balance can`t be a negative."));
                return;
            }

            _money = moneyAtStart;
            _id = ID;
        }

        public decimal Balance
        {
            get => _money;
            private set => _money = value;
        }

        public void SpendMoney(ActivityOrProduct activity)
        {
            if (_money < activity.Price)
            {
                Logs.LogException(new InvalidAmountException("Negative Balance"));
                InfoAboutDeal.LogToFile(_id, "Not enough money. You tried to spend on " + activity.Name +
                                             " about ", activity.Price, this);
                InvalidCardOperation?.Invoke(this, new WalletHandlerArgs("You can`t spend money, " +
                                                                         "your balance is negative"));
                return;
            }

            _money -= activity.Price;
            InfoAboutDeal.LogToFile(_id, activity.Name, activity.Price, this);
        }

        public void GetMoney(decimal sum)
        {
            if (sum > Balance)
            {
                Logs.LogException(new InvalidAmountException("You don`t have enough money."));
                InvalidCardOperation?.Invoke(this, new WalletHandlerArgs("Недостаточно денег на счету."));
                return;
            }

            if (sum < 0)
            {
                Logs.LogException(new InvalidAmountException("You can`t get a negative amount of money."));
                return;
            }

            _money -= sum;
            InfoAboutDeal.LogToFile(_id, "You withdrew ", sum, this);
        }

        public void AddMoney(ActivityOrProduct activity)
        {
            if (activity.Price < 0)
            {
                Logs.LogException(new InvalidAmountException("Negative price detected"));
                InvalidCardOperation?.Invoke(this, new WalletHandlerArgs("Price can`t be a negative."));
                return;
            }

            Balance += activity.Price;
            InfoAboutDeal.LogToFile(_id, $"Received from \'{activity.Name}\'", activity.Price, this);
            SuccessfulOperation?.Invoke(this,
                new WalletHandlerArgs($"On \'{_id}\' card was received {activity.Price} from {activity.Name}"));
        }

        private event CardsOperation InvalidCardOperation;
        private event CardsOperation SuccessfulOperation;

        // Function that merge two bank accounts.
        public static BankAccount operator +(BankAccount fst, BankAccount scd)
        {
            fst = new BankAccount(fst.Balance + scd.Balance, fst._id);
            var temp = scd.Balance;
            scd.Balance = 0;
            fst.SuccessfulOperation(fst,
                new WalletHandlerArgs($"All money from \'{scd._id}\' were transferred to \'{fst._id}\'"));
            InfoAboutDeal.LogToFile(fst._id, $"Received from \'{scd._id}\' card", temp, fst);
            InfoAboutDeal.LogToFile(scd._id, $"Transferred to \'{fst._id}\' card", temp, scd);
            return fst;
        }

        public static void Transfer(BankAccount from, BankAccount to, decimal sum)
        {
            if (sum < 0)
            {
                Logs.LogException(new InvalidAmountException("Not enough money."));
                from.InvalidCardOperation?.Invoke(from, new WalletHandlerArgs("Not enough money."));
                return;
            }

            if (sum > from.Balance)
            {
                InfoAboutDeal.LogToFile(from._id, $"You tried to transfer to {to._id} card ", sum, from);
                from.InvalidCardOperation?.Invoke(from, new WalletHandlerArgs("Not enough money."));
                return;
            }

            to.Balance += sum;
            from.Balance -= sum;
            from.SuccessfulOperation(from,
                new WalletHandlerArgs($"From \'{from._id}\' about ${sum} was transferred to \'{to._id}\'."));
            InfoAboutDeal.LogToFile(to._id, $"Received from {from._id}", sum, to);
            InfoAboutDeal.LogToFile(from._id, $"Transferred to {from._id} about", sum, from);
        }

        private static void Errors(object sender, WalletHandlerArgs handler)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(handler.Message);
            Console.ResetColor();
        }

        private static void Success(object sender, WalletHandlerArgs handler)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Error.WriteLine(handler.Message);
            Console.ResetColor();
        }

        private delegate void CardsOperation(object sender, WalletHandlerArgs handler);
    }
}