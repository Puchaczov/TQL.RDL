using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class NoneToken : Token
    {
        public NoneToken() 
            : base(string.Empty, SyntaxType.None, new TextSpan(0,0))
        { }
    }
}
