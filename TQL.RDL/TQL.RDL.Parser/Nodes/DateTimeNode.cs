using System;
using System.Globalization;
using RDL.Parser.Tokens;

namespace RDL.Parser.Nodes
{
    public class DateTimeNode : LeafNode
    {
        private readonly CultureInfo _ci;
        private readonly string[] _formats;
        private TimeSpan _zone;

        public DateTimeNode(Token token, TimeSpan zone, string[] formats, CultureInfo ci) 
            : base(token)
        {
            _zone = zone;
            _formats = formats;
            _ci = ci;
        }

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[0];

        public DateTimeOffset DateTime => DateTimeOffset.ParseExact(Token.Value, _formats, _ci, DateTimeStyles.None);

        public override Type ReturnType => typeof(DateTimeOffset);

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}
