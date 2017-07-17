using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class BetweenAndToken : Token
    {
        public BetweenAndToken(TextSpan span)
            : base(string.Empty, StatementType.BetweenAnd, span)
        {
        }
    }
}