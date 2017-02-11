using TQL.Core.Tokens;

namespace RDL.Parser.Tokens
{
    public class BetweenAndToken : Token
    {
        public BetweenAndToken(TextSpan span)
            : base(string.Empty, StatementType.BetweenAnd, span)
        {
        }
    }
}