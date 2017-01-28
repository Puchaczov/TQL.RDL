using System;
using RDL.Parser.Tokens;

namespace RDL.Parser.Nodes
{
    public class NotNode : UnaryNode
    {
        public NotNode(Token token, RdlSyntaxNode node) : base(node)
        {
            Token = token;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override Token Token { get; }
        public override Type ReturnType => typeof(Boolean);
    }
}
