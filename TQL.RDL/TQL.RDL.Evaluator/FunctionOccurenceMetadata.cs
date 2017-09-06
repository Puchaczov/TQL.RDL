using System.Collections.Generic;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator
{
    public class FunctionOccurenceMetadata
    {
        private List<RawFunctionNode> Nodes { get; }

        public FunctionOccurenceMetadata()
        {
            Nodes = new List<RawFunctionNode>();
        }

        public int Count => Nodes.Count;
    }
}
