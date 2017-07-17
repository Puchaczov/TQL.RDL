using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class ModuloToken : Token
    {
        public const string TokenText = "%";

        public ModuloToken(TextSpan span)
            : base(TokenText, StatementType.Mod, span)
        {
        }
    }
}