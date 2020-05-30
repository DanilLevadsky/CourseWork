using System;

namespace LogsAndExceptions
{
    public class InvalidAmountException : ArgumentException
    {
        public InvalidAmountException(string message) : base(message)
        {
        }

    }
}