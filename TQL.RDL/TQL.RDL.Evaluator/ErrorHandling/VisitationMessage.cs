using System.Collections.Generic;
using TQL.Core.Tokens;

namespace TQL.RDL.Evaluator.ErrorHandling
{
    public abstract class VisitationMessage
    {
        private readonly TextSpan[] _spans;

        protected VisitationMessage(TextSpan[] spans, string message)
        {
            this.Message = message;
            this._spans = spans;
        }

        /// <summary>
        /// Represents message that should will be returned to user.
        /// </summary>
        protected string Message { get; }

        /// <summary>
        /// Significance of message.
        /// </summary>
        public abstract MessageLevel Level { get; }

        /// <summary>
        /// Code of that message
        /// </summary>
        public abstract Codes Code { get; }

        /// <summary>
        /// Places in text where these items refer.
        /// </summary>
        public IReadOnlyCollection<TextSpan> Spans => _spans;

        /// <summary>
        /// Stringify object.
        /// </summary>
        /// <returns></returns>
        public abstract override string ToString();
    }
}