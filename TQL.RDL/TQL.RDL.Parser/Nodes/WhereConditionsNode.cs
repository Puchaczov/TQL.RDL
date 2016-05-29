using System;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class WhereConditionsNode : UnaryNode
    {
        public WhereConditionsNode(RdlSyntaxNode node) 
            : base(node)
        { }

        public override Type ReturnType => typeof(bool);

        public override Token Token => null;

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}
