using TQL.Core.Tokens;

namespace RDL.Parser.Tokens
{
    public class RightParenthesisToken : Token
    {
        public const string TokenText = ")";

        public RightParenthesisToken(TextSpan textSpan)
            : base(TokenText, StatementType.RightParenthesis, textSpan)
        { }
    }
}
