using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class EndOfFileToken : Token
    {
        public const string TokenText = "eof";

        public EndOfFileToken(TextSpan span)
            : base(string.Empty, StatementType.EndOfFile, span)
        {
        }
    }
}