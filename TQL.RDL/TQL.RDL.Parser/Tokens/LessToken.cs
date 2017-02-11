using TQL.Core.Tokens;

namespace RDL.Parser.Tokens
{
    public class LessToken : Token
    {
        public const string TokenText = "<";

        public LessToken(TextSpan span)
            : base(TokenText, StatementType.Less, span)
        {
        }
    }
}