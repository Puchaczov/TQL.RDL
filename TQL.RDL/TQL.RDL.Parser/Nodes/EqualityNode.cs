using System;

namespace TQL.RDL.Parser.Nodes
{
    public class EqualityNode : BinaryNode
    {
        public EqualityNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        {
        }

        public override Type ReturnType => typeof(bool);

        public override string ToString() => ToString("=");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}