using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class WhiteSpaceToken : Token
    {
        public WhiteSpaceToken(TextSpan span)
            : base(TokenText, StatementType.WhiteSpace, span)
        { }

        public const string TokenText = " ";
    }
}
