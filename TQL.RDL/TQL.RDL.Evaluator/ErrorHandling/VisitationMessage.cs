using System.Collections.Generic;
using TQL.Core.Tokens;

namespace TQL.RDL.Evaluator.ErrorHandling
{
    public abstract class VisitationMessage
    {
        private readonly string _message;
        private readonly TextSpan[] _spans;

        protected VisitationMessage(TextSpan[] spans, string message)
        {
            this._message = message;
            this._spans = spans;
        }

        public string Message => _message;

        public abstract MessageLevel Level { get; }
        public abstract Codes Code { get; }
        public IReadOnlyCollection<TextSpan> Spans => _spans;
        public abstract override string ToString();
    }
}