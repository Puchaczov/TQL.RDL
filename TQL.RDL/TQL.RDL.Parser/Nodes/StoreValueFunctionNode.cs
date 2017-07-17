using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class StoreValueFunctionNode : RawFunctionNode
    {
        public StoreValueFunctionNode(RawFunctionNode node)
            : base(node.Token as FunctionToken, node.Args, node.ReturnType, node.DoNotCache)
        {
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}