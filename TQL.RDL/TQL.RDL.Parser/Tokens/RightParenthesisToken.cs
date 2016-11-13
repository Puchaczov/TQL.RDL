using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class RightParenthesisToken : Token
    {
        public RightParenthesisToken(TextSpan textSpan)
            : base(TokenText, StatementType.RightParenthesis, textSpan)
        { }

        public const string TokenText = ")";
    }
}
