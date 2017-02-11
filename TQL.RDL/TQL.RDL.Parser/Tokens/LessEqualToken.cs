using TQL.Core.Tokens;

namespace RDL.Parser.Tokens
{
    public class LessEqualToken : Token
    {
        public const string TokenText = "<=";

        public LessEqualToken(TextSpan span)
            : base(TokenText, StatementType.LessEqual, span)
        {
        }
    }
}