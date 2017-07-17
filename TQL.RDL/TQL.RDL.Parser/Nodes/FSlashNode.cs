namespace TQL.RDL.Parser.Nodes
{
    public class FSlashNode : BinaryNode
    {
        public FSlashNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        {
        }

        public override string ToString() => ToString("/");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}