namespace TQL.RDL.Parser.Nodes
{
    public class StarNode : BinaryNode
    {
        public StarNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        {
        }

        public override string ToString() => ToString("*");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}