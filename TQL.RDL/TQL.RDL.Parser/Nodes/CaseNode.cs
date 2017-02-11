using System;
using System.Linq;
using RDL.Parser.Tokens;
using TQL.Core.Tokens;

namespace RDL.Parser.Nodes
{
    public class CaseNode : RdlSyntaxNode
    {
        private readonly Token _caseToken;

        public CaseNode(Token caseToken, WhenThenNode[] nodes, ElseNode node)
        {
            node.SetParent(this);

            Expressions = nodes;
            Else = node;

            foreach (var item in nodes)
                item.SetParent(this);

            _caseToken = caseToken;

            Descendants = Expressions.Concat(new RdlSyntaxNode[1] {Else}).ToArray();
        }

        public override RdlSyntaxNode[] Descendants { get; }

        public RdlSyntaxNode[] WhenThenExpressions => Expressions;

        public override TextSpan FullSpan
            => new TextSpan(_caseToken.Span.Start, Else.FullSpan.End - _caseToken.Span.Start);

        public override bool IsLeaf => false;

        public override Type ReturnType => Expressions[0].ReturnType;

        public WhenThenNode[] Expressions { get; }

        public ElseNode Else { get; }

        public override Token Token => _caseToken;

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override int GetHashCode() => ToString().GetHashCode();
    }
}