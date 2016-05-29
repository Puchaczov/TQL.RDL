using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class RightParenthesisToken : Token
    {
        public RightParenthesisToken(TextSpan textSpan)
            : base(")", SyntaxType.RightParenthesis, textSpan)
        { }
    }
}
