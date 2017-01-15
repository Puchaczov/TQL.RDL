using System;

namespace TQL.RDL.Evaluator.ErrorHandling
{
    public class FatalVisitError : VisitationMessage
    {
        private readonly Exception _exc;

        public FatalVisitError(Exception exc)
            : base(null, exc.Message)
        {
            _exc = exc;
        }

        public override Codes Code => Codes.C01;
        public override MessageLevel Level => MessageLevel.Error;

        public override string ToString() => Message;
    }
}
