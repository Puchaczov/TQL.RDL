using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class RightParenthesisToken : Token
    {
        public const string TokenText = ")";

        public RightParenthesisToken(TextSpan textSpan)
            : base(TokenText, StatementType.RightParenthesis, textSpan)
        {
        }
    }
}