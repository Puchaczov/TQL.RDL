using System;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class ThenNode : UnaryNode
    {
        public ThenNode(Token token, RdlSyntaxNode then)
            : base(then)
        {
            Token = token;
        }

        public override Type ReturnType => Descendant.ReturnType;

        public override Token Token { get; }

        public WhenThenNode Parent { get; private set; }

        public void SetParent(WhenThenNode parent)
        {
            Parent = parent;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString() => $"then {Descendants[0]}";
    }
}