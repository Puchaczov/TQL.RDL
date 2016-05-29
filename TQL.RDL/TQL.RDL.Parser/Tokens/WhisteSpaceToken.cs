using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class WhisteSpaceToken : Token
    {
        public WhisteSpaceToken(TextSpan span)
            : base(" ", SyntaxType.WhiteSpace, span)
        { }
    }
}
