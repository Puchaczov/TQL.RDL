using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class WordToken : Token
    {
        public WordToken(string value, TextSpan span)
            : base(value, SyntaxType.Word, span)
        { }
    }
}
