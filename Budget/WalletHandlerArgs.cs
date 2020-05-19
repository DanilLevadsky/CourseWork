namespace Budget
{
    public class WalletHandlerArgs
    {
        private string _message;

        public string Message => _message;

        public WalletHandlerArgs(string message)
        {
            _message = message;
        }
    }
}