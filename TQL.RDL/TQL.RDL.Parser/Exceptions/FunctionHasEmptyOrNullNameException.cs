using System;

namespace RDL.Parser.Exceptions
{
    internal class FunctionHasEmptyOrNullNameException : Exception
    {
        public FunctionHasEmptyOrNullNameException()
        {
        }

        public FunctionHasEmptyOrNullNameException(string message) : base(message)
        {
        }

        public FunctionHasEmptyOrNullNameException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}