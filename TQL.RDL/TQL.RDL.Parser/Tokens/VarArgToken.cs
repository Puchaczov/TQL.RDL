using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    public class VarArgToken : Token
    {
        public const string TokenText = "arg";

        public VarArgToken(int argsCount)
            : base("arg", StatementType.VarArg, new TextSpan(0, 0))
        {
            Arguments = argsCount;
        }

        public VarArgToken(string name)
            : base(name, StatementType.VarArg, new TextSpan(0, 0))
        {
        }

        public int Arguments { get; }
    }
}