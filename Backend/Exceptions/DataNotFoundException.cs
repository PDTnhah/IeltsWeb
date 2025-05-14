using System;

namespace Backend.Exceptions
{
    public class DataNotFoundException : Exception
    {
        public DataNotFoundException(string message) : base(message)
        {
        }
    }
}