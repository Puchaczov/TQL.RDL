using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class GreaterToken : Token
    {
        public const string TokenText = ">";

        public GreaterToken(TextSpan span)
            : base(TokenText, StatementType.Greater, span)
        {
        }
    }
}