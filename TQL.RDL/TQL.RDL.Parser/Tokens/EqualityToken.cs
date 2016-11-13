using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class EqualityToken : Token
    {
        public EqualityToken(TextSpan span)
            : base(TokenText, StatementType.Equality, span)
        { }

        public const string TokenText = "=";
    }
}
