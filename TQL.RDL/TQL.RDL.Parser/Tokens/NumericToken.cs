using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class NumericToken : Token
    {
        public NumericToken(string value, TextSpan span)
            : base(value, SyntaxType.Numeric, span)
        { }
    }
}
