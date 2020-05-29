namespace Budget
{
    internal interface IWallet
    {
        internal void SpendMoney(ActivityOrProduct activity);
        internal void AddMoney(ActivityOrProduct activity);
        internal void GetMoney(decimal sum);
    }
}