using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class EveryToken : Token
    {
        public const string TokenText = "every";

        public EveryToken(TextSpan span)
            : base(TokenText, StatementType.Every, span)
        {
        }
    }
}