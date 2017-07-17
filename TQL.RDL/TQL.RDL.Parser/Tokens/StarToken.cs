using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class StarToken : Token
    {
        public const string TokenText = "*";

        public StarToken(TextSpan span)
            : base(TokenText, StatementType.Star, span)
        {
        }
    }
}