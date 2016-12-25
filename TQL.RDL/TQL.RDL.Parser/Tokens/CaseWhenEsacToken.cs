using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    internal class CaseWhenEsacToken : Token
    {
        private TextSpan textSpan;

        public CaseWhenEsacToken(string value, TextSpan textSpan)
            : base(value, StatementType.CaseWhenEsac, textSpan)
        {
            this.textSpan = textSpan;
        }
    }
}