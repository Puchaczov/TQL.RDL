using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class LeftParenthesisToken : Token
    {
        public const string TokenText = "(";

        public LeftParenthesisToken(TextSpan textSpan)
            : base(TokenText, StatementType.LeftParenthesis, textSpan)
        {
        }
    }
}