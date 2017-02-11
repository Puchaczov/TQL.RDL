using TQL.Core.Tokens;

namespace RDL.Parser.Tokens
{
    public class NotToken : Token
    {
        public const string TokenText = "not";

        public NotToken(TextSpan span)
            : base(TokenText, StatementType.Not, span)
        {
        }
    }
}