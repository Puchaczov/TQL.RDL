using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class EqualityToken : Token
    {
        public EqualityToken(TextSpan span)
            : base("=", SyntaxType.Equality, span)
        { }
    }
}
