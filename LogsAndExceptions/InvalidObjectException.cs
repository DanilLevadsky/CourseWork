using System;

namespace LogsAndExceptions
{
    public class InvalidObjectException : ArgumentException
    {
        public InvalidObjectException(string message) : base(message)
        {
        }
    }
}