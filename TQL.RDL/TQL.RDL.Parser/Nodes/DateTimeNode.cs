using System;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class DateTimeNode : LeafNode
    {
        public DateTimeNode(Token token) 
            : base(token)
        { }

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[0];

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public DateTimeOffset DateTime => DateTimeOffset.Parse(base.Token.Value);

        public override Type ReturnType => typeof(DateTimeOffset);
    }
}
