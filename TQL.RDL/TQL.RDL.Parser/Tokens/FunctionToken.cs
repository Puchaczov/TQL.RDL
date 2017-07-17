using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class FunctionToken : Token
    {
        public const string TokenText = "function";

        public FunctionToken(string fname, TextSpan span)
            : base(fname, StatementType.Function, span)
        {
        }

        public override string ToString() => Value;
    }
}