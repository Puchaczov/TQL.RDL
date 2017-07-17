using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class NoneToken : Token
    {
        public const string TokenText = "none";

        public NoneToken()
            : base(string.Empty, StatementType.None, new TextSpan(0, 0))
        {
        }
    }
}