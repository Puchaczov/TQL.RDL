using System;

namespace TQL.RDL.Exceptions
{
    internal class InjectParameterCannotBeOptionalException : Exception
    {
        public InjectParameterCannotBeOptionalException()
        {
        }

        public InjectParameterCannotBeOptionalException(string message) : base(message)
        {
        }

        public InjectParameterCannotBeOptionalException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}