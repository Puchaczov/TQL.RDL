using TQL.Core.Tokens;

namespace RDL.Parser.Tokens
{
    public class HyphenToken : Token
    {
        public const string TokenText = "-";

        public HyphenToken(TextSpan span)
            : base(TokenText, StatementType.Hyphen, span)
        {
        }
    }
}