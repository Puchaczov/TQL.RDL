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

        public override string ToString() => $"{Left} {"in"} ({Right})";
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}