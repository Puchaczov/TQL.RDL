using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class WhereToken : Token
    {
        public const string TokenText = "where";

        public WhereToken(TextSpan span)
            : base(TokenText, StatementType.Where, span)
        {
        }
    }
}