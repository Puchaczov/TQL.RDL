using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class FSlashToken : Token
    {
        public const string TokenText = "/";

        public FSlashToken(TextSpan span)
            : base(TokenText, StatementType.FSlash, span)
        {
        }
    }
}