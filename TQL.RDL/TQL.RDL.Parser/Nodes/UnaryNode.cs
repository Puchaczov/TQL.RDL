using TQL.Core.Tokens;

namespace RDL.Parser.Nodes
{
    public abstract class UnaryNode : RdlSyntaxNode
    {
        private readonly RdlSyntaxNode _node;

        protected UnaryNode(RdlSyntaxNode node)
        {
            _node = node;
        }

        public override bool IsLeaf => false;

        public override RdlSyntaxNode[] Descendants => new[] { _node };

        public virtual RdlSyntaxNode Descendant => Descendants[0];

        public override TextSpan FullSpan => Descendant.FullSpan.Clone();

        public override string ToString() => _node.ToString();
    }
}
