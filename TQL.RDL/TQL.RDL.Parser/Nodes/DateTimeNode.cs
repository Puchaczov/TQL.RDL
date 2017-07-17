using System;
using System.Globalization;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class DateTimeNode : LeafNode
    {
        private readonly CultureInfo _ci;
        private readonly string[] _formats;

        public DateTimeNode(Token token, string[] formats, CultureInfo ci)
            : base(token)
        {
            _formats = formats;
            _ci = ci;
        }

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[0];

        public DateTimeOffset DateTime
        {
            get
            {
                var dt = DateTimeOffset.ParseExact(Token.Value, _formats, _ci, DateTimeStyles.None);
                return new DateTimeOffset(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, TimeSpan.Zero);
            }
        }

        public override Type ReturnType => typeof(DateTimeOffset);

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}