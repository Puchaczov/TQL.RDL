using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class DiffToken : Token
    {
        public DiffToken(TextSpan span)
            : base("<>", SyntaxType.Diff, span)
        { }
    }
}
