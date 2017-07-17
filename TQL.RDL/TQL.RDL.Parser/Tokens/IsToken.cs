using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class IsToken : Token
    {
        public const string TokenText = "is";

        public IsToken(TextSpan span)
            : base(TokenText, StatementType.Is, span)
        {
        }
    }
}