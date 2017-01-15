using System.Collections.Generic;
using TQL.Core.Tokens;

namespace TQL.RDL.Evaluator.ErrorHandling
{
    public class SemanticError : VisitationMessage
    {
        private static readonly Dictionary<SemanticErrorKind, Codes> Codes;

        static SemanticError()
        {
            Codes = new Dictionary<SemanticErrorKind, Codes> {{SemanticErrorKind.MixedValues, ErrorHandling.Codes.C02}};
        }

        public SemanticError(TextSpan span, string message, SemanticErrorKind kind)
            : base(new[] { span }, message)
        {
            Kind = kind;
        }

        public SemanticError(TextSpan[] spans, string message, SemanticErrorKind kind)
            : base(spans, message)
        {
            Kind = kind;
        }

        public override MessageLevel Level => MessageLevel.Error;
        private SemanticErrorKind Kind { get; }

        public override Codes Code => Codes[Kind];
        public override string ToString() => Message;
    }
}