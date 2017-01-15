using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class AndToken : Token
    {
        public const string TokenText = "and";

        public AndToken(TextSpan span)
            : base(TokenText, StatementType.And, span)
        { }
    }

    public class OrToken : Token
    {
        public const string TokenText = "or";

        public OrToken(TextSpan span)
            : base(TokenText, StatementType.Or, span)
        { }
    }

    public class WhereToken : Token
    {
        public const string TokenText = "where";

        public WhereToken(TextSpan span)
            : base(TokenText, StatementType.Where, span)
        { }
    }

    public class InToken : Token
    {
        public const string TokenText = "in";

        public InToken(TextSpan span)
            : base(TokenText, StatementType.In, span)
        { }
    }

    public class IsToken : Token
    {
        public const string TokenText = "is";

        public IsToken(TextSpan span)
            : base(TokenText, StatementType.Is, span)
        { }
    }

    public class NotInToken : Token
    {
        public const string TokenText = "not in";

        public NotInToken(TextSpan span)
            : base(TokenText, StatementType.NotIn, span)
        { }
    }

    public class NotToken : Token
    {
        public const string TokenText = "not";

        public NotToken(TextSpan span)
            : base(TokenText, StatementType.Not, span)
        { }
    }

    public class EveryToken : Token
    {
        public const string TokenText = "every";

        public EveryToken(TextSpan span)
            : base(TokenText, StatementType.Every, span)
        { }
    }

    public class PlusToken : Token
    {
        public const string TokenText = "+";

        public PlusToken(TextSpan span)
            : base(TokenText, StatementType.Plus, span)
        { }
    }

    public class ModuloToken : Token
    {
        public const string TokenText = "%";

        public ModuloToken(TextSpan span)
            : base(TokenText, StatementType.Mod, span)
        { }
    }

    public class StarToken : Token
    {
        public const string TokenText = "*";

        public StarToken(TextSpan span)
            : base(TokenText, StatementType.Star, span)
        { }
    }

    public class FSlashToken : Token
    {
        public const string TokenText = "/";

        public FSlashToken(TextSpan span)
            : base(TokenText, StatementType.FSlash, span)
        { }
    }

    public class HyphenToken : Token
    {
        public const string TokenText = "-";

        public HyphenToken(TextSpan span)
            : base(TokenText, StatementType.Hyphen, span)
        { }
    }

    public class GreaterToken : Token
    {
        public const string TokenText = ">";

        public GreaterToken(TextSpan span)
            : base(TokenText, StatementType.Greater, span)
        { }
    }

    public class GreaterEqualToken : Token
    {
        public const string TokenText = ">=";

        public GreaterEqualToken(TextSpan span)
            : base(TokenText, StatementType.GreaterEqual, span)
        { }
    }

    public class LessToken : Token
    {
        public const string TokenText = "<";

        public LessToken(TextSpan span)
            : base(TokenText, StatementType.Less, span)
        { }
    }

    public class LessEqualToken : Token
    {
        public const string TokenText = "<=";

        public LessEqualToken(TextSpan span)
            : base(TokenText, StatementType.LessEqual, span)
        { }
    }

    public class RepeatToken : Token
    {
        public const string TokenText = "repeat";

        public RepeatToken(TextSpan span)
            : base(TokenText, StatementType.Repeat, span)
        { }
    }

    public class StartAtToken : Token
    {
        public const string TokenText = "start at";

        public StartAtToken(TextSpan span)
            : base(TokenText, StatementType.StartAt, span)
        { }
    }

    public class StopAtToken : Token
    {
        public const string TokenText = "stop at";

        public StopAtToken(TextSpan span)
            : base(TokenText, StatementType.StopAt, span)
        { }
    }

    public class VarToken : Token
    {
        public const string TokenText = "var";

        public VarToken(string value, TextSpan span)
            : base(value, StatementType.Var, span)
        { }

        public override string ToString() => string.Format("@{0}", Value);
    }

    public class VarArgToken : Token
    {
        public const string TokenText = "arg";

        public VarArgToken(int argsCount)
            : base("arg", StatementType.VarArg, new TextSpan(0, 0))
        {
            Arguments = argsCount;
        }

        public VarArgToken(string name)
            : base(name, StatementType.VarArg, new TextSpan(0, 0))
        { }

        public int Arguments { get; }
    }

    public class FunctionToken : Token
    {
        public const string TokenText = "function";

        public FunctionToken(string fname, TextSpan span)
            : base(fname, StatementType.Function, span)
        { }

        public override string ToString() => Value;
    }

    public class CaseToken : Token
    {
        public const string TokenText = "case";

        public CaseToken(TextSpan span)
            : base(TokenText, StatementType.Case, span)
        { }
    }

    public class WhenToken : Token
    {
        public const string TokenText = "when";

        public WhenToken(TextSpan span)
            : base(TokenText, StatementType.When, span)
        { }
    }

    public class ThenToken : Token
    {
        public const string TokenText = "then";

        public ThenToken(TextSpan span)
            : base(TokenText, StatementType.Then, span)
        { }
    }

    public class ElseToken : Token
    {
        public const string TokenText = "else";

        public ElseToken(TextSpan span)
            : base(TokenText, StatementType.Else, span)
        { }
    }

    public class CaseEndToken : Token
    {
        public const string TokenText = "esac";

        public CaseEndToken(TextSpan span)
            : base(TokenText, StatementType.CaseEnd, span)
        { }
    }
}
