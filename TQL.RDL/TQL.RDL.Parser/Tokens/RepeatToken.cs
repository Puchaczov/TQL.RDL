using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class RepeatToken : Token
    {
        public const string TokenText = "repeat";

        public RepeatToken(TextSpan span)
            : base(TokenText, StatementType.Repeat, span)
        {
        }
    }
}