using System.Collections.Generic;
using TQL.Core.Tokens;

namespace TQL.RDL.Evaluator.ErrorHandling
{
    public class SyntaxError : VisitationMessage
    {
        private static readonly Dictionary<SyntaxErrorKind, Codes> Codes;

        static SyntaxError()
        {
            Codes = new Dictionary<SyntaxErrorKind, Codes>
            {
                {SyntaxErrorKind.MissingValue, ErrorHandling.Codes.C02},
                {SyntaxErrorKind.ImproperType, ErrorHandling.Codes.C02},
                {SyntaxErrorKind.UnsupportedFunctionCall, ErrorHandling.Codes.C02},
                {SyntaxErrorKind.WrongKeyword, ErrorHandling.Codes.C02}
            };
        }

        private SyntaxError(TextSpan[] spans, string message, SyntaxErrorKind kind)
            : base(spans, message)
        {
            Kind = kind;
        }

        public SyntaxError(TextSpan span, string message, SyntaxErrorKind kind)
            : this(new[] { span }, message, kind)
        { }

        public override MessageLevel Level => MessageLevel.Error;
        public SyntaxErrorKind Kind { get; }

        public override Codes Code => Codes[Kind];

        public override string ToString() => Message;
    }
}