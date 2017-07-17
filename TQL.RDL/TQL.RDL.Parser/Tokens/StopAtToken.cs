using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class StopAtToken : Token
    {
        public const string TokenText = "stop at";

        public StopAtToken(TextSpan span)
            : base(TokenText, StatementType.StopAt, span)
        {
        }
    }
}