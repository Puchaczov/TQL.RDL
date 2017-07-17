using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class BetweenToken : Token
    {
        public const string TokenText = "between";

        public BetweenToken(TextSpan span)
            : base(string.Empty, StatementType.Between, span)
        {
        }
    }
}