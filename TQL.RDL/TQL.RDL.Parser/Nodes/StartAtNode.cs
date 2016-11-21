using System;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class StartAtNode : ConstantNode
    {
        private LeafNode when;
        private Token token;

        public override Type ReturnType => typeof(bool);

        public StartAtNode(Token token, LeafNode when) 
            : base(when)
        {
            this.token = token;
            this.when = when;
        }

        public override string ToString() => string.Format("start at {0}", when);

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public DateTimeOffset When => (when as DateTimeNode).DateTime;
    }
}
