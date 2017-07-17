using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class AndToken : Token
    {
        public const string TokenText = "and";

        public AndToken(TextSpan span)
            : base(TokenText, StatementType.And, span)
        {
        }
    }
}