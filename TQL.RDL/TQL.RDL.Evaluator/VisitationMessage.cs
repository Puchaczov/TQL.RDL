using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TQL.Core.Tokens;

namespace TQL.RDL.Evaluator
{
    public enum SyntaxErrorKind
    {
        MissingValue,
        WrongKeyword,
        UnsupportedFunctionCall
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
        private readonly SyntaxErrorKind kind;

        private static readonly Dictionary<SyntaxErrorKind, Codes> codes;

        static SyntaxError()
        {
            codes = new Dictionary<SyntaxErrorKind, Codes>();
            codes.Add(SyntaxErrorKind.MissingValue, Codes.C02);
        }

        public SyntaxError(TextSpan[] spans, string message, SyntaxErrorKind kind)
            : base(spans, message)
        {
            this.kind = kind;
        }

        public SyntaxError(TextSpan span, string message, SyntaxErrorKind kind)
            : this(new TextSpan[] { span }, message, kind)
        { }

        public override MessageLevel Level => MessageLevel.Error;
        public SyntaxErrorKind Kind => kind;
        public override Codes Code => codes[kind];

        public override string ToString() => message;
    }

    public class SemanticError : VisitationMessage
    {
        private readonly SemanticErrorKind kind;
        private static readonly Dictionary<SemanticErrorKind, Codes> codes;

        static SemanticError()
        {
            codes = new Dictionary<SemanticErrorKind, Codes>();
            codes.Add(SemanticErrorKind.MixedValues, Codes.C02);
        }

        public SemanticError(TextSpan span, string message, SemanticErrorKind kind)
            : base(new TextSpan[] { span }, message)
        {
            this.kind = kind;
        }

        public SemanticError(TextSpan[] spans, string message, SemanticErrorKind kind)
            : base(spans, message)
        {
            this.kind = kind;
        }

        public override MessageLevel Level => MessageLevel.Error;
        public SemanticErrorKind Kind => kind;
        public override Codes Code => codes[kind];
        public override string ToString() => this.message;
    }

    public class FatalVisitError : VisitationMessage
    {
        private readonly Exception exc;

        public FatalVisitError(Exception exc)
            : base(null, exc.Message)
        {
            this.exc = exc;
        }

        public override Codes Code => Codes.C01;
        public override MessageLevel Level => MessageLevel.Error;

        public override string ToString() => this.message;
    }
}
