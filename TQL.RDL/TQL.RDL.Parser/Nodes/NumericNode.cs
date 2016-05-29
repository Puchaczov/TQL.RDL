using System;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class NumericNode : LeafNode
    {
        public NumericNode(Token token) 
            : base(token)
        { }

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[0];

        public override Type ReturnType => typeof(long);

        public int Value => int.Parse(Token.Value);

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}
