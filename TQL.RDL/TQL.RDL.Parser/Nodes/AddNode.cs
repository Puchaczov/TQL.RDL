namespace RDL.Parser.Nodes
{
    public class AddNode : BinaryNode
    {
        public AddNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        {
        }

        public override string ToString() => ToString("+");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}