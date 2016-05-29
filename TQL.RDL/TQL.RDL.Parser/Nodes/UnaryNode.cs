using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public abstract class UnaryNode : RdlSyntaxNode
    {
        private RdlSyntaxNode node;

        public UnaryNode(RdlSyntaxNode node)
        {
            this.node = node;
        }

        public override bool IsLeaf => false;

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[] { node };

        public virtual RdlSyntaxNode Descendant => Descendants[0];

        public override TextSpan FullSpan => Descendant.FullSpan.Clone();

        public override string ToString() => node.ToString();
    }
}
