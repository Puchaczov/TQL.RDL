using System.Collections.Generic;
using RDL.Parser.Nodes;

namespace TQL.RDL.Converter
{
    internal struct ParseResult
    {
        public RootScriptNode Root { get; }
        public IDictionary<int, int> FunctionCallOccurences { get; }

        public ParseResult(RootScriptNode root, IDictionary<int, int> functionCallOccurences)
        {
            Root = root;
            FunctionCallOccurences = functionCallOccurences;
        }
    }
}