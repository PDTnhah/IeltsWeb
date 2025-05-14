using System;

namespace Backend.Exceptions
{
    public class InvalidParamException : Exception
    {
        public InvalidParamException(string message) : base(message)
        {
        }
    }
}