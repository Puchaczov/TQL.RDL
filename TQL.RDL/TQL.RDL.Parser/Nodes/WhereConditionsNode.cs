using System;
using RDL.Parser.Tokens;

namespace RDL.Parser.Nodes
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
