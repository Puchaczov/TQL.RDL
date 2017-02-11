using TQL.Core.Tokens;

namespace RDL.Parser.Tokens
{
    public class WordToken : Token
    {
        public const string TokenText = "word";

        public WordToken(string value, TextSpan span)
            : base(value, StatementType.Word, span)
        {
        }
    }
}