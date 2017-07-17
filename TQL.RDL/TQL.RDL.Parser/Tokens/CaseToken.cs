using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class CaseToken : Token
    {
        public const string TokenText = "case";

        public CaseToken(TextSpan span)
            : base(TokenText, StatementType.Case, span)
        {
        }
    }
}