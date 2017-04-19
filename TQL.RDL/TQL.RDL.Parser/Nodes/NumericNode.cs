using System;
using RDL.Parser.Tokens;

namespace RDL.Parser.Nodes
{
    public class NumericNode : LeafNode
    {
        public NumericNode(Token token, Type inferedType)
            : base(token)
        {
            ReturnType = inferedType;
        }

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[0];

        public override Type ReturnType { get; }

        public long Value => long.Parse(Token.Value);

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}