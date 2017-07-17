using System;

namespace TQL.RDL.Parser.Nodes
{
    public class OrNode : BinaryNode
    {
        public OrNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        {
        }

        public override Type ReturnType => typeof(bool);

        public override string ToString() => ToString("or");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}