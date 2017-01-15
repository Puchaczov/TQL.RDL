using System;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class StartAtNode : ConstantNode
    {
        private Token _token;
        private readonly LeafNode _when;

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
