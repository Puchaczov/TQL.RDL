using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class HyphenNode : BinaryNode
    {
        public HyphenNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        {
        }

        public override string ToString() => ToString("-");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
        public override Token Token => new Token("-", StatementType.Hyphen, default(TextSpan));
    }
}