using System;

namespace LogsAndExceptions
{
    public class InvalidCardOperationException : ArgumentException
    {
        public InvalidCardOperationException(string message) : base(message)
        {
        }
    }
}