using System;

namespace TQL.RDL.Parser.Nodes
{
    public class InNode : BinaryNode
    {
        public InNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        {
        }

        public override Type ReturnType => typeof(bool);

        public override string ToString() => string.Format("{0} {1} ({2})", Left, "in", Right);
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}