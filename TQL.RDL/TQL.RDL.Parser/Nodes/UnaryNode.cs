using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public abstract class UnaryNode : RdlSyntaxNode
    {
        private RdlSyntaxNode _node;

        public UnaryNode(RdlSyntaxNode node)
        {
            _node = node;
        }

        public override bool IsLeaf => false;

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[] { _node };

        public virtual RdlSyntaxNode Descendant => Descendants[0];

        public override TextSpan FullSpan => Descendant.FullSpan.Clone();

        public override string ToString() => _node.ToString();
    }
}
