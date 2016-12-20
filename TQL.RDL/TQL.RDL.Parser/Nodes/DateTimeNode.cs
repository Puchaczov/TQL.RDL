using System;
using System.Globalization;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class DateTimeNode : LeafNode
    {
        private TimeSpan zone;
        private string[] formats;
        private CultureInfo ci;

        public DateTimeNode(Token token, TimeSpan zone, string[] formats, CultureInfo ci) 
            : base(token)
        {
            this.zone = zone;
            this.formats = formats;
            this.ci = ci;
        }

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[0];

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public DateTimeOffset DateTime => DateTimeOffset.ParseExact(base.Token.Value, formats, ci, DateTimeStyles.None);

        public override Type ReturnType => typeof(DateTimeOffset);
    }
}
