using System.Collections.Generic;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL
{
    internal struct ParseResult
    {
        private RootScriptNode Root { get; }
        private IDictionary<int, int> FunctionCallOccurences { get; }

        public ParseResult(RootScriptNode root, IDictionary<int, int> functionCallOccurences)
        {
            Root = root;
            FunctionCallOccurences = functionCallOccurences;
        }
    }
}