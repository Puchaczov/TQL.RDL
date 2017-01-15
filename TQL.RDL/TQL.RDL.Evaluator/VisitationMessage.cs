using System;
using System.Collections.Generic;
using TQL.Core.Tokens;

namespace TQL.RDL.Evaluator
{
    public enum SyntaxErrorKind
    {
        MissingValue,
        WrongKeyword,
        UnsupportedFunctionCall,
        ImproperType,
        LackOfExpression
    }

    public enum SemanticErrorKind
    {
        MixedValues
    }

    public enum MessageLevel
    {
        Info,
        Warning,
        Error
    }

    public enum Codes : uint
    {
        C01,
        C02,
        C03,
        C04,
        C05,
        C06
    }

    public abstract class VisitationMessage
    {
        protected readonly string message;
        protected readonly TextSpan[] spans;

        protected VisitationMessage(TextSpan[] spans, string message)
        {
            this.message = message;
            this.spans = spans;
        }

        public string Message => message;

        public abstract MessageLevel Level { get; }
        public abstract Codes Code { get; }
        public IReadOnlyCollection<TextSpan> Spans => spans;
        public abstract override string ToString();
    }

    public class SyntaxError : VisitationMessage
    {
        private static readonly Dictionary<SyntaxErrorKind, Codes> Codes;

        static SyntaxError()
        {
            Codes = new Dictionary<SyntaxErrorKind, Codes>();
            Codes.Add(SyntaxErrorKind.MissingValue, Evaluator.Codes.C02);
            Codes.Add(SyntaxErrorKind.ImproperType, Evaluator.Codes.C02);
            Codes.Add(SyntaxErrorKind.UnsupportedFunctionCall, Evaluator.Codes.C02);
            Codes.Add(SyntaxErrorKind.WrongKeyword, Evaluator.Codes.C02);
        }

        public SyntaxError(TextSpan[] spans, string message, SyntaxErrorKind kind)
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

        public override string ToString() => message;
    }

    public class SemanticError : VisitationMessage
    {
        private static readonly Dictionary<SemanticErrorKind, Codes> Codes;

        static SemanticError()
        {
            Codes = new Dictionary<SemanticErrorKind, Codes>();
            Codes.Add(SemanticErrorKind.MixedValues, Evaluator.Codes.C02);
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
        public SemanticErrorKind Kind { get; }

        public override Codes Code => Codes[Kind];
        public override string ToString() => message;
    }

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

        public override string ToString() => message;
    }
}
