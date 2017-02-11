using System.Collections.Generic;
using TQL.Core.Tokens;

namespace TQL.RDL.Evaluator.ErrorHandling
{
    public class SemanticError : VisitationMessage
    {
        private static readonly Dictionary<SemanticErrorKind, Codes> Codes;

        /// <summary>
        ///     Initialize static fields.
        /// </summary>
        static SemanticError()
        {
            Codes = new Dictionary<SemanticErrorKind, Codes> {{SemanticErrorKind.MixedValues, ErrorHandling.Codes.C02}};
        }

        /// <summary>
        ///     Initialize object.
        /// </summary>
        /// <param name="span">Text position where such error occur.</param>
        /// <param name="message">Message that identify that error.</param>
        /// <param name="kind">Kind of error</param>
        public SemanticError(TextSpan span, string message, SemanticErrorKind kind)
            : base(new[] {span}, message)
        {
            Kind = kind;
        }

        /// <summary>
        ///     Initialize object
        /// </summary>
        /// <param name="spans">Positions where such error occur.</param>
        /// <param name="message">Message that identify that error.</param>
        /// <param name="kind">Kind of error.</param>
        public SemanticError(TextSpan[] spans, string message, SemanticErrorKind kind)
            : base(spans, message)
        {
            Kind = kind;
        }

        /// <summary>
        ///     Level of significance (Error)
        /// </summary>
        public override MessageLevel Level => MessageLevel.Error;

        /// <summary>
        ///     Kind of information
        /// </summary>
        private SemanticErrorKind Kind { get; }

        /// <summary>
        ///     Information level
        /// </summary>
        public override Codes Code => Codes[Kind];

        /// <summary>
        ///     Stringify object.
        /// </summary>
        /// <returns>string representation of object.</returns>
        public override string ToString() => $"{Kind}/{Message}";
    }
}