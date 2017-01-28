using System;
using RDL.Parser.Tokens;

namespace RDL.Parser.Nodes
{
    public class StopAtNode : ConstantNode
    {
        private Token _token;
        private readonly DateTimeNode _when;

        public StopAtNode(Token token, DateTimeNode when) 
            : base(when)
        {
            _token = token;
            _when = when;
        }

        public override Type ReturnType => typeof(DateTimeOffset);

        public DateTimeOffset When => _when.DateTime;

        public override string ToString() => $"stop at '{_when}'";

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}
