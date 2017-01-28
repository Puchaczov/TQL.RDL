using System;
using RDL.Parser.Tokens;

namespace RDL.Parser.Nodes
{
    public class WordNode : LeafNode
    {
        public WordNode(Token token)
            : base(token)
        { }

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[0];

        public override Type ReturnType => typeof(string);

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}
