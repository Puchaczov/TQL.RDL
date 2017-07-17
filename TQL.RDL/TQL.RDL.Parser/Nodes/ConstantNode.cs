using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public abstract class ConstantNode : RdlSyntaxNode
    {
        public ConstantNode(LeafNode node)
        {
            Value = node;
        }

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[] {Value};

        public override TextSpan FullSpan => Value.FullSpan;

        public override bool IsLeaf => true;

        public override Token Token => null;

        public LeafNode Value { get; }

        public override void Accept(INodeVisitor visitor)
        {
        }
    }
}