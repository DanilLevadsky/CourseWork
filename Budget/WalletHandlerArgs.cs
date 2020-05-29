namespace Budget
{
    internal class WalletHandlerArgs
    {
        internal WalletHandlerArgs(string message)
        {
            Message = message;
        }

        internal string Message { get; }
    }
}