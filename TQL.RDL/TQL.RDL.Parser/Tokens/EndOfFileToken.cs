using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class EndOfFileToken : Token
    {
        public EndOfFileToken(TextSpan span)
            : base(string.Empty, StatementType.EndOfFile, span)
        { }

        public const string TokenText = "eof";
    }
}
