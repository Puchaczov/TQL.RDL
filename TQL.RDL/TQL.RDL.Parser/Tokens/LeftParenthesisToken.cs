using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class LeftParenthesisToken : Token
    {
        public LeftParenthesisToken(TextSpan textSpan)
            : base("(", SyntaxType.LeftParenthesis, textSpan)
        { }
    }
}
