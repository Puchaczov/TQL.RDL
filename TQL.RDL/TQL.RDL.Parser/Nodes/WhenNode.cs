using System;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class WhenNode : UnaryNode
    {
        public WhenNode(Token token, RdlSyntaxNode when)
            : base(when)
        {
            Token = token;
        }

        private RdlSyntaxNode Expression => Descendant;

        public override Type ReturnType => Descendant.ReturnType;

        public override Token Token { get; }

        public WhenThenNode Parent { get; private set; }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void SetParent(WhenThenNode node)
        {
            Parent = node;
        }

        public override string ToString() => $"when {Expression}";
    }
}