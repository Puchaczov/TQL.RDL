using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class DiffToken : Token
    {
        public DiffToken(TextSpan span)
            : base(TokenText, StatementType.Diff, span)
        { }

        public const string TokenText = "<>";
    }
}
