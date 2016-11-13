using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class WordToken : Token
    {
        public WordToken(string value, TextSpan span)
            : base(value, StatementType.Word, span)
        { }

        public const string TokenText = "word";
    }
}
