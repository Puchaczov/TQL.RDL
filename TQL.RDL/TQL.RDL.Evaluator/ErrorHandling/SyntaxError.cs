using System.Collections.Generic;
using TQL.Core.Tokens;

namespace TQL.RDL.Evaluator.ErrorHandling
{
    public class SyntaxError : VisitationMessage
    {
        private static readonly Dictionary<SyntaxErrorKind, Codes> Codes;

        /// <summary>
        ///     Initialize static fields.
        /// </summary>
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

        /// <summary>
        ///     Initialize object
        /// </summary>
        /// <param name="spans">Positions where such error occur.</param>
        /// <param name="message">Message that identify that error.</param>
        /// <param name="kind">Kind of error.</param>
        private SyntaxError(TextSpan[] spans, string message, SyntaxErrorKind kind)
            : base(spans, message)
        {
            Kind = kind;
        }

        /// <summary>
        ///     Initialize object.
        /// </summary>
        /// <param name="span">Text position where such error occur.</param>
        /// <param name="message">Message that identify that error.</param>
        /// <param name="kind">Kind of error</param>
        public SyntaxError(TextSpan span, string message, SyntaxErrorKind kind)
            : this(new[] {span}, message, kind)
        {
        }

        /// <summary>
        ///     Significance level of message.
        /// </summary>
        public override MessageLevel Level => MessageLevel.Error;

        /// <summary>
        ///     Kind of message.
        /// </summary>
        public SyntaxErrorKind Kind { get; }

        /// <summary>
        ///     Code of message.
        /// </summary>
        public override Codes Code => Codes[Kind];

        /// <summary>
        ///     Stringify object.
        /// </summary>
        /// <returns>string representation of object.</returns>
        public override string ToString() => $"{Kind}/{Message}";
    }
}