using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class ElseToken : Token
    {
        public const string TokenText = "else";

        public ElseToken(TextSpan span)
            : base(TokenText, StatementType.Else, span)
        {
        }
    }
}