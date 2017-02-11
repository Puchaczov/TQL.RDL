using System;
using RDL.Parser.Tokens;

namespace RDL.Parser.Nodes
{
    public class StartAtNode : ConstantNode
    {
        private readonly LeafNode _when;
        private Token _token;

        public StartAtNode(Token token, LeafNode when)
            : base(when)
        {
            _token = token;
            _when = when;
        }

        public override Type ReturnType => typeof(bool);

        public DateTimeOffset When => (_when as DateTimeNode).DateTime;

        public override string ToString() => string.Format("start at {0}", _when);

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}