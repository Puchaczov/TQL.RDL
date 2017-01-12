using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public abstract class ConstantNode : RdlSyntaxNode
    {
        private LeafNode _value;
        
        public ConstantNode(LeafNode node)
        {
            _value = node;
        }

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[] { _value };

        public override TextSpan FullSpan => _value.FullSpan;

        public override bool IsLeaf => true;

        public override Token Token => null;

        public override void Accept(INodeVisitor visitor) { }

        public LeafNode Value => _value;
    }
}
