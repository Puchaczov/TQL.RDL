using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class NotInToken : Token
    {
        public const string TokenText = "not in";

        public NotInToken(TextSpan span)
            : base(TokenText, StatementType.NotIn, span)
        {
        }
    }
}