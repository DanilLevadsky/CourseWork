namespace Budget
{
    public abstract class Wallet
    {
        protected decimal _money = 0;

        protected Wallet(decimal moneyAtStart = 0)
        {
            this._money += moneyAtStart;
        }
    }
}