using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class CaseEndToken : Token
    {
        public const string TokenText = "esac";

        public CaseEndToken(TextSpan span)
            : base(TokenText, StatementType.CaseEnd, span)
        {
        }
    }
}