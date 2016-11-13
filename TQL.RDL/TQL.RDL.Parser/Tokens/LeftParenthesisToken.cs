using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class LeftParenthesisToken : Token
    {
        public LeftParenthesisToken(TextSpan textSpan)
            : base(TokenText, StatementType.LeftParenthesis, textSpan)
        { }

        public const string TokenText = "(";
    }
}
