using TQL.Core.Tokens;

namespace RDL.Parser.Tokens
{
    public class StartAtToken : Token
    {
        public const string TokenText = "start at";

        public StartAtToken(TextSpan span)
            : base(TokenText, StatementType.StartAt, span)
        {
        }
    }
}