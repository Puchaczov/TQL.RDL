using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class CommaToken : Token
    {
        public CommaToken(TextSpan span)
            : base(",", SyntaxType.Comma, span)
        { }
    }
}
