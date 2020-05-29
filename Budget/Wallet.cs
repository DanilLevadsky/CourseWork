namespace Budget
{
    public abstract class Wallet
    {
        protected decimal _money;

        protected Wallet(decimal moneyAtStart = 0)
        {
            _money += moneyAtStart;
        }
    }
}