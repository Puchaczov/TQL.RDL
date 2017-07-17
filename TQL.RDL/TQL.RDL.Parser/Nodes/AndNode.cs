using System;

namespace TQL.RDL.Parser.Nodes
{
    public class AndNode : BinaryNode
    {
        public AndNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        {
        }

        public override Type ReturnType => typeof(bool);

        public override string ToString() => ToString("and");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}