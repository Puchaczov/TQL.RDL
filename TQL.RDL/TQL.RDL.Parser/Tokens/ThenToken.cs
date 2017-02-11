using TQL.Core.Tokens;

namespace RDL.Parser.Tokens
{
    public class ThenToken : Token
    {
        public const string TokenText = "then";

        public ThenToken(TextSpan span)
            : base(TokenText, StatementType.Then, span)
        {
        }
    }
}