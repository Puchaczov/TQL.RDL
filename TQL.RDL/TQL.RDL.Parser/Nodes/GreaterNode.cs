using System;

namespace TQL.RDL.Parser.Nodes
{
    public class GreaterNode : BinaryNode
    {
        public GreaterNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        {
        }

        public override Type ReturnType => typeof(bool);

        public override string ToString() => ToString(">");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}