using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class GreaterEqualToken : Token
    {
        public const string TokenText = ">=";

        public GreaterEqualToken(TextSpan span)
            : base(TokenText, StatementType.GreaterEqual, span)
        {
        }
    }
}