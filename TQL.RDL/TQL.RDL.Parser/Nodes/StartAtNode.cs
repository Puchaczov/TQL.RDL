using System;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class StartAtNode : ConstantNode
    {
        private readonly LeafNode _when;

        public StartAtNode(LeafNode when)
            : base(when)
        {
            _when = when;
        }

        public override Type ReturnType => typeof(bool);

        public DateTimeOffset When => (_when as DateTimeNode).DateTime;

        public override string ToString() => $"start at {_when}";

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}