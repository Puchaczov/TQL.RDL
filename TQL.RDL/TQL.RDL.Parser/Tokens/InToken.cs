using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class InToken : Token
    {
        public const string TokenText = "in";

        public InToken(TextSpan span)
            : base(TokenText, StatementType.In, span)
        {
        }
    }
}