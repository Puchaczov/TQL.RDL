using TQL.Core.Tokens;

namespace RDL.Parser.Tokens
{
    public class OrToken : Token
    {
        public const string TokenText = "or";

        public OrToken(TextSpan span)
            : base(TokenText, StatementType.Or, span)
        {
        }
    }
}