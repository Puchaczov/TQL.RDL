using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public abstract class ConstantNode : RdlSyntaxNode
    {
        private LeafNode value;
        
        public ConstantNode(LeafNode node)
        {
            this.value = node;
        }

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[] { value };

        public override TextSpan FullSpan => value.FullSpan;

        public override bool IsLeaf => true;

        public override Token Token => null;

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public LeafNode Value => value;
    }
}
