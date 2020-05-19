namespace Budget
{
    public abstract class Wallet
    {
        protected decimal _money = 0;

        protected Wallet(decimal moneyAtStart)
        {
            this._money += moneyAtStart;
        }

        public abstract void SpendMoney(ActivityOrProduct activity);
        public abstract void AddMoney(ActivityOrProduct activity);
        public abstract void GetMoney(decimal sum);
    }
}