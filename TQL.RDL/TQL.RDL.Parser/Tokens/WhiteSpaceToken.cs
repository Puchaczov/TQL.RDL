using TQL.Core.Tokens;

namespace RDL.Parser.Tokens
{
    public class WhiteSpaceToken : Token
    {
        public const string TokenText = " ";

        public WhiteSpaceToken(TextSpan span)
            : base(TokenText, StatementType.WhiteSpace, span)
        {
        }
    }
}