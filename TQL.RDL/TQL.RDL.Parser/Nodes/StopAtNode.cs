using System;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class StopAtNode : ConstantNode
    {
        private LeafNode when;
        private Token token;

        public StopAtNode(Token token, LeafNode when) 
            : base(when)
        {
            this.token = token;
            this.when = when;
        }

        public override string ToString() => string.Format("`stop at` {0}", when);

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override Type ReturnType => typeof(DateTimeOffset);

    }
}
