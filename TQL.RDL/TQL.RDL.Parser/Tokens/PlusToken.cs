using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class PlusToken : Token
    {
        public const string TokenText = "+";

        public PlusToken(TextSpan span)
            : base(TokenText, StatementType.Plus, span)
        {
        }
    }
}