using System;

namespace OrderProcessing.Domain.Exceptions
{
    public class InvalidProductCodeException : Exception
    {
        public InvalidProductCodeException(string message) : base(message)
        {
        }
    }
}
