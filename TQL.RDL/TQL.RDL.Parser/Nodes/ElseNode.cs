using System;
using RDL.Parser.Tokens;

namespace RDL.Parser.Nodes
{
    public class ElseNode : UnaryNode
    {
        public ElseNode(Token token, RdlSyntaxNode node)
            : base(node)
        {
            Token = token;
        }

        public override Type ReturnType => Descendant.ReturnType;

        public override Token Token { get; }

        public CaseNode Parent { get; private set; }

        public void SetParent(CaseNode parent)
        {
            Parent = parent;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString() => string.Format("else {0}", Descendants[0]);
    }
}