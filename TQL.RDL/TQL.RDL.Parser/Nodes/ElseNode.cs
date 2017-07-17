using System;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
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

        public override string ToString() => $"else {Descendants[0]}";
    }
}