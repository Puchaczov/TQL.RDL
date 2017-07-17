using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class WhenToken : Token
    {
        public const string TokenText = "when";

        public WhenToken(TextSpan span)
            : base(TokenText, StatementType.When, span)
        {
        }
    }
}