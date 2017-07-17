using System;

namespace TQL.RDL.Exceptions
{
    internal class InjectParameterHasDefaultValueException : Exception
    {
        public InjectParameterHasDefaultValueException()
        {
        }

        public InjectParameterHasDefaultValueException(string message) : base(message)
        {
        }

        public InjectParameterHasDefaultValueException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}