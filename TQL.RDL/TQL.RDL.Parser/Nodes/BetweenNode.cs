using System;
using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class BetweenNode : RdlSyntaxNode
    {
        public BetweenNode(Token betweenToken, RdlSyntaxNode exp, RdlSyntaxNode minNode, RdlSyntaxNode maxNode)
        {
            Descendants = new[] {exp, minNode, maxNode};
            Token = betweenToken;
        }

        public override RdlSyntaxNode[] Descendants { get; }

        public RdlSyntaxNode Expression => Descendants[0];

        public RdlSyntaxNode Min => Descendants[1];

        public RdlSyntaxNode Max => Descendants[2];

        public override TextSpan FullSpan
            => new TextSpan(Expression.FullSpan.Start, Max.FullSpan.End - Expression.FullSpan.Start);

        public override bool IsLeaf => false;

        public override Type ReturnType => typeof(bool);

        public override Token Token { get; }

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString() => $"{Expression} between {Min} and {Max}";
    }
}