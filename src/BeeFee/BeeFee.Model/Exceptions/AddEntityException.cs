using System;

namespace BeeFee.Model.Exceptions
{
    public class AddEntityException : Exception
    {
        public AddEntityException() : base()
        {
        }

        public AddEntityException(string message) : base(message)
        {
        }

        public AddEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}