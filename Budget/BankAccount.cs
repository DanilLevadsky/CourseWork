using System;
using LogsAndExceptions;

namespace Budget
{
    public sealed class BankAccount : Wallet, IWallet
    {
        public string _id;

        public BankAccount(decimal moneyAtStart, string ID) : base(moneyAtStart)
        {
            InvalidCardOperation = Errors;
            SuccessfulOperation = Success;
            if (moneyAtStart < 0)
            {
                InvalidCardOperation?.Invoke(this, new WalletHandlerArgs("Балланс не может быть отрицательным"));
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
                InfoAboutDeal.LogToFile(_id, "Недостаточно денег. Вы попытались потратить на " + activity.Name +
                                             " сумму в ", activity.Price, this);
                InvalidCardOperation?.Invoke(this, new WalletHandlerArgs("Вы не можете потратить деньги, " +
                                                                         "у вас на счету нет средств"));
                throw new InvalidCardOperationException("Недостаточно денег");
            }

            _money -= activity.Price;
            InfoAboutDeal.LogToFile(_id, activity.Name, activity.Price, this);
        }

        public void GetMoney(decimal sum)
        {
            if (sum > Balance)
            {
                InvalidCardOperation?.Invoke(this, new WalletHandlerArgs("Недостаточно денег на счету."));
                throw new InvalidCardOperationException("Недостаточно денег на счету");
            }

            if (sum < 0)
            {
                throw new InvalidCardOperationException("Нельзя вывести отрицательную сумму");
            }

            _money -= sum;
            InfoAboutDeal.LogToFile(_id, "You withdrew ", sum, this);
        }

        public void AddMoney(ActivityOrProduct activity)
        {
            if (activity.Price < 0)
            {
                InvalidCardOperation?.Invoke(this, new WalletHandlerArgs("Цена не может быть отрицательной"));
                throw new InvalidCardOperationException("Цена не может быть отрицательной");
            }

            Balance += activity.Price;
            InfoAboutDeal.LogToFile(_id, $"Получено от \'{activity.Name}\'", activity.Price, this);
            SuccessfulOperation?.Invoke(this,
                new WalletHandlerArgs($"На карту \'{_id}\' было получено {activity.Price} от {activity.Name}"));
        }

        private event CardsOperation InvalidCardOperation;
        private event CardsOperation SuccessfulOperation;


        public static void Transfer(BankAccount from, BankAccount to, decimal sum)
        {
            if (sum < 0)
            {
                from.InvalidCardOperation?.Invoke(from, new WalletHandlerArgs("Недостаточно денег"));
                throw new InvalidCardOperationException("Недостаточно денег");
            }

            if (sum > from.Balance)
            {
                InfoAboutDeal.LogToFile(from._id, $"Вы попытались перенести на карту \'{to._id}\'", sum, from);
                from.InvalidCardOperation?.Invoke(from, new WalletHandlerArgs("Недостаточно денег."));
                return;
            }

            to.Balance += sum;
            from.Balance -= sum;
            from.SuccessfulOperation(from,
                new WalletHandlerArgs($"От \'{from._id}\' было переведено ${sum} на \'{to._id}\'."));
            InfoAboutDeal.LogToFile(to._id, $"Получено от {from._id}", sum, to);
            InfoAboutDeal.LogToFile(from._id, $"Переведено на \'{from._id}\' ", sum, from);
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