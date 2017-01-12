using System;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class StopAtNode : ConstantNode
    {
        private DateTimeNode _when;
        private Token _token;

        public StopAtNode(Token token, DateTimeNode when) 
            : base(when)
        {
            _token = token;
            _when = when;
        }

        public override string ToString() => string.Format("stop at '{0}'", _when);

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override Type ReturnType => typeof(DateTimeOffset);

        public DateTimeOffset When => _when.DateTime;

    }
}
