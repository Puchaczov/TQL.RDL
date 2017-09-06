namespace TQL.RDL.Parser.Nodes
{
    public class CommaNode : BinaryNode
    {
        public CommaNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        {
        }

        public override string ToString() => $"{Left},{Right}";

        public override void Accept(INodeVisitor visitor)
        {
        }
    }
}