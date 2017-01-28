using System;
using RDL.Parser.Tokens;

namespace RDL.Parser.Nodes
{
    public class NumericNode : LeafNode
    {
        public NumericNode(Token token) 
            : base(token)
        { }

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[0];

        public override Type ReturnType => typeof(long);

        //TO DO: should be long??
        public int Value => int.Parse(Token.Value);

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}
