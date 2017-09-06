using System;

namespace TQL.RDL.Parser.Nodes
{
    public class NotInNode : InNode
    {
        public NotInNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        {
        }

        public override Type ReturnType => typeof(bool);

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString() => $"not in {Right}";
    }
}