using System;
using System.Collections.Generic;
using System.Text;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator
{
    public class FunctionOccurenceMetadata
    {
        public List<RawFunctionNode> Nodes { get; }

        public FunctionOccurenceMetadata()
        {
            Nodes = new List<RawFunctionNode>();
        }

        public int Count => Nodes.Count;
    }
}
