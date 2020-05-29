namespace Budget
{
    internal interface IWallet
    {
        public void SpendMoney(ActivityOrProduct activity);
        public void AddMoney(ActivityOrProduct activity);
        public void GetMoney(decimal sum);
    }
}