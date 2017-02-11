using System;
using RDL.Parser.Tokens;

namespace RDL.Parser.Nodes
{
    public class WhenNode : UnaryNode
    {
        public WhenNode(Token token, RdlSyntaxNode when)
            : base(when)
        {
            Token = token;
        }

        public RdlSyntaxNode Expression => Descendant;

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

        public override string ToString() => string.Format("when {0}", Expression);
    }
}