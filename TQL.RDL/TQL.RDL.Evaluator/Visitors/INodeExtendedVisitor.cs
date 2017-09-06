using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Visitors
{
    public interface INodeExtendedVisitor : INodeVisitor
    {
        void Visit(RawFunctionNode node, FunctionOccurenceMetadata metadatas);
        void Visit(CallFunctionAndStoreValueNode node, FunctionOccurenceMetadata metadatas);
    }
}
