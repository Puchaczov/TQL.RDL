using System;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class WordNode : LeafNode
    {
        public WordNode(Token token)
            : base(token)
        {
        }

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[0];

        public override Type ReturnType => typeof(string);

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}