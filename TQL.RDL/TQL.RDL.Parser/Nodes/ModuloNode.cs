namespace TQL.RDL.Parser.Nodes
{
    public class ModuloNode : BinaryNode
    {
        public ModuloNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        {
        }

        public override string ToString() => ToString("%");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}