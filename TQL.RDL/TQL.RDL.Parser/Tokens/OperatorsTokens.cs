using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class AndToken : Token
    {
        public AndToken(TextSpan span)
            : base("and", SyntaxType.And, span)
        { }
    }

    public class OrToken : Token
    {
        public OrToken(TextSpan span)
            : base("or", SyntaxType.Or, span)
        { }
    }

    public class WhereToken : Token
    {
        public WhereToken(TextSpan span)
            : base("where", SyntaxType.Where, span)
        { }
    }

    public class InToken : Token
    {
        public InToken(TextSpan span)
            : base("in", SyntaxType.In, span)
        { }
    }

    public class NotInToken : Token
    {
        public NotInToken(TextSpan span)
            : base("not in", SyntaxType.NotIn, span)
        { }
    }

    public class EveryToken : Token
    {
        public EveryToken(TextSpan span)
            : base("every", SyntaxType.Every, span)
        { }
    }

    public class PlusToken : Token
    {
        public PlusToken(TextSpan span)
            : base("+", SyntaxType.Plus, span)
        { }
    }

    public class ModuloToken : Token
    {
        public ModuloToken(TextSpan span)
            : base("%", SyntaxType.Mod, span)
        { }
    }

    public class StarToken : Token
    {
        public StarToken(TextSpan span)
            : base("*", SyntaxType.Star, span)
        { }
    }

    public class FSlashToken : Token
    {
        public FSlashToken(TextSpan span)
            : base("/", SyntaxType.FSlash, span)
        { }
    }

    public class HyphenToken : Token
    {
        public HyphenToken(TextSpan span)
            : base("-", SyntaxType.Hyphen, span)
        { }
    }

    public class GreaterToken : Token
    {
        public GreaterToken(TextSpan span)
            : base(">", SyntaxType.Greater, span)
        { }
    }

    public class GreaterEqualToken : Token
    {
        public GreaterEqualToken(TextSpan span)
            : base(">=", SyntaxType.GreaterEqual, span)
        { }
    }

    public class LessToken : Token
    {
        public LessToken(TextSpan span)
            : base("<", SyntaxType.Less, span)
        { }
    }

    public class LessEqualToken : Token
    {
        public LessEqualToken(TextSpan span)
            : base("<=", SyntaxType.LessEqual, span)
        { }
    }

    public class RepeatToken : Token
    {
        public RepeatToken(TextSpan span)
            : base("repeat", SyntaxType.Repeat, span)
        { }
    }

    public class StartAtToken : Token
    {
        public StartAtToken(TextSpan span)
            : base("start at", SyntaxType.StartAt, span)
        { }
    }

    public class StopAtToken : Token
    {
        public StopAtToken(TextSpan span)
            : base("stop at", SyntaxType.StopAt, span)
        { }
    }

    public class VarToken : Token
    {
        public VarToken(TextSpan span, string value)
            : base(value, SyntaxType.Var, span)
        { }

        public override string ToString() => string.Format("@{0}", Value);
    }

    public class VarArgToken : Token
    {
        public VarArgToken(int argsCount)
            : base("arg", SyntaxType.VarArg, new TextSpan(0, 0))
        {
            Arguments = argsCount;
        }

        public VarArgToken(string name)
            : base(name, SyntaxType.VarArg, new TextSpan(0, 0))
        { }

        public int Arguments { get; }
    }

    public class FunctionToken : Token
    {
        public FunctionToken(string fname, TextSpan span)
            : base(fname, SyntaxType.Function, span)
        { }

        public override string ToString() => Value;
    }
}
