using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class AndToken : Token
    {
        public AndToken(TextSpan span)
            : base(TokenText, StatementType.And, span)
        { }

        public const string TokenText = "and";
    }

    public class OrToken : Token
    {
        public OrToken(TextSpan span)
            : base(TokenText, StatementType.Or, span)
        { }

        public const string TokenText = "or";
    }

    public class WhereToken : Token
    {
        public WhereToken(TextSpan span)
            : base(TokenText, StatementType.Where, span)
        { }

        public const string TokenText = "where";
    }

    public class InToken : Token
    {
        public InToken(TextSpan span)
            : base(TokenText, StatementType.In, span)
        { }

        public const string TokenText = "in";
    }

    public class IsToken : Token
    {
        public IsToken(TextSpan span)
            : base(TokenText, StatementType.Is, span)
        { }

        public const string TokenText = "is";
    }

    public class NotInToken : Token
    {
        public NotInToken(TextSpan span)
            : base(TokenText, StatementType.NotIn, span)
        { }

        public const string TokenText = "not in";
    }

    public class NotToken : Token
    {
        public NotToken(TextSpan span)
            : base(TokenText, StatementType.Not, span)
        { }

        public const string TokenText = "not";
    }

    public class EveryToken : Token
    {
        public EveryToken(TextSpan span)
            : base(TokenText, StatementType.Every, span)
        { }

        public const string TokenText = "every";
    }

    public class PlusToken : Token
    {
        public PlusToken(TextSpan span)
            : base(TokenText, StatementType.Plus, span)
        { }

        public const string TokenText = "+";
    }

    public class ModuloToken : Token
    {
        public ModuloToken(TextSpan span)
            : base(TokenText, StatementType.Mod, span)
        { }

        public const string TokenText = "%";
    }

    public class StarToken : Token
    {
        public StarToken(TextSpan span)
            : base(TokenText, StatementType.Star, span)
        { }

        public const string TokenText = "*";
    }

    public class FSlashToken : Token
    {
        public FSlashToken(TextSpan span)
            : base(TokenText, StatementType.FSlash, span)
        { }

        public const string TokenText = "/";
    }

    public class HyphenToken : Token
    {
        public HyphenToken(TextSpan span)
            : base(TokenText, StatementType.Hyphen, span)
        { }

        public const string TokenText = "-";
    }

    public class GreaterToken : Token
    {
        public GreaterToken(TextSpan span)
            : base(TokenText, StatementType.Greater, span)
        { }

        public const string TokenText = ">";
    }

    public class GreaterEqualToken : Token
    {
        public GreaterEqualToken(TextSpan span)
            : base(TokenText, StatementType.GreaterEqual, span)
        { }

        public const string TokenText = ">=";
    }

    public class LessToken : Token
    {
        public LessToken(TextSpan span)
            : base(TokenText, StatementType.Less, span)
        { }

        public const string TokenText = "<";
    }

    public class LessEqualToken : Token
    {
        public LessEqualToken(TextSpan span)
            : base(TokenText, StatementType.LessEqual, span)
        { }

        public const string TokenText = "<=";
    }

    public class RepeatToken : Token
    {
        public RepeatToken(TextSpan span)
            : base(TokenText, StatementType.Repeat, span)
        { }

        public const string TokenText = "repeat";
    }

    public class StartAtToken : Token
    {
        public StartAtToken(TextSpan span)
            : base(TokenText, StatementType.StartAt, span)
        { }

        public const string TokenText = "start at";
    }

    public class StopAtToken : Token
    {
        public StopAtToken(TextSpan span)
            : base(TokenText, StatementType.StopAt, span)
        { }

        public const string TokenText = "stop at";
    }

    public class VarToken : Token
    {
        public VarToken(string value, TextSpan span)
            : base(value, StatementType.Var, span)
        { }

        public override string ToString() => string.Format("@{0}", Value);

        public const string TokenText = "var";
    }

    public class VarArgToken : Token
    {
        public VarArgToken(int argsCount)
            : base("arg", StatementType.VarArg, new TextSpan(0, 0))
        {
            Arguments = argsCount;
        }

        public VarArgToken(string name)
            : base(name, StatementType.VarArg, new TextSpan(0, 0))
        { }

        public int Arguments { get; }

        public const string TokenText = "arg";
    }

    public class FunctionToken : Token
    {
        public FunctionToken(string fname, TextSpan span)
            : base(fname, StatementType.Function, span)
        { }

        public override string ToString() => Value;

        public const string TokenText = "function";
    }

    public class CaseToken : Token
    {
        public CaseToken(TextSpan span)
            : base(TokenText, StatementType.Case, span)
        { }

        public const string TokenText = "case";
    }

    public class WhenToken : Token
    {
        public WhenToken(TextSpan span)
            : base(TokenText, StatementType.When, span)
        { }

        public const string TokenText = "when";
    }

    public class ThenToken : Token
    {
        public ThenToken(TextSpan span)
            : base(TokenText, StatementType.Then, span)
        { }

        public const string TokenText = "then";
    }

    public class ElseToken : Token
    {
        public ElseToken(TextSpan span)
            : base(TokenText, StatementType.Else, span)
        { }

        public const string TokenText = "else";
    }

    public class CaseEndToken : Token
    {
        public CaseEndToken(TextSpan span)
            : base(TokenText, StatementType.CaseEnd, span)
        { }

        public const string TokenText = "esac";
    }
}
