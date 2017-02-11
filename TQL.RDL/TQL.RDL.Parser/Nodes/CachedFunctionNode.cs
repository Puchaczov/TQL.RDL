using RDL.Parser.Tokens;

namespace RDL.Parser.Nodes
{
    public class CachedFunctionNode : RawFunctionNode
    {
        public CachedFunctionNode(RawFunctionNode node)
            : base(node.Token as FunctionToken, node.Args, node.ReturnType)
        {
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}