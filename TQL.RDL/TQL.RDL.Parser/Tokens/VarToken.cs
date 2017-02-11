using TQL.Core.Tokens;

namespace RDL.Parser.Tokens
{
    public class VarToken : Token
    {
        public const string TokenText = "var";

        public VarToken(string value, TextSpan span)
            : base(value, StatementType.Var, span)
        {
        }

        public override string ToString() => string.Format("@{0}", Value);
    }
}