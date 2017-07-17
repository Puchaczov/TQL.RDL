using System.Collections.Generic;
using TQL.RDL.Parser.Helpers;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Visitors
{
    public class ExtendedTraverser : Traverser
    {
        private readonly IDictionary<int, int> _methodOccurences;

        /// <summary>
        /// Initialize instance.
        /// </summary>
        /// <param name="codeGenerationVisitor">The destination visitor.</param>
        /// <param name="methodOccurences">Method occurences dictionary.</param>
        public ExtendedTraverser(INodeVisitor codeGenerationVisitor, IDictionary<int, int> methodOccurences)
            : base(codeGenerationVisitor)
        {
            _methodOccurences = methodOccurences;
        }

        /// <summary>
        ///     Visit Function node in DFS manner.
        /// </summary>
        /// <param name="node">Function node that will be visited.</param>
        public override void Visit(RawFunctionNode node)
        {
            if (WillFunctionCallOccurencesAtLeastTwoTimes(node) && IsCacheable(node))
            {
                if (!CanBeRetrievedFromCache(node))
                {
                    AddFunctionOccurence(node);
                    Visit(new StoreValueFunctionNode(node));
                }
                else
                {
                    Visit(new CachedFunctionNode(node));
                }
            }
            else
                base.Visit(node);
        }

        /// <summary>
        /// Determine if function is cacheable.
        /// </summary>
        /// <param name="node">The raw function.</param>
        /// <returns>True if node is cacheable, else false.</returns>
        private bool IsCacheable(RawFunctionNode node)
        {
            return !node.DoNotCache;
        }

        /// <summary>
        ///     Determine if function call occured at least twice.
        /// </summary>
        /// <param name="node">The function node.</param>
        /// <returns>True if function call occurs at least twice, otherwise false.</returns>
        private bool WillFunctionCallOccurencesAtLeastTwoTimes(RawFunctionNode node)
        {
            var fnKey = node.Stringify().GetHashCode();

            if (!_methodOccurences.ContainsKey(fnKey))
                return false;

            return _methodOccurences[fnKey] > 1;
        }

        /// <summary>
        ///     Add processed function as processed earlier.
        /// </summary>
        /// <param name="node"></param>
        private void AddFunctionOccurence(RawFunctionNode node)
        {
            if (!OccurenceTable.ContainsKey(node.Name))
                OccurenceTable.Add(node.Name, new List<RawFunctionNode>());

            OccurenceTable[node.Name].Add(node);
        }

        /// <summary>
        ///     Determine if function node were processed earlier and if can be restored from cache.
        /// </summary>
        /// <param name="node">The Function node.</param>
        /// <returns>True if function can be restored from cache, else false.</returns>
        private bool CanBeRetrievedFromCache(RawFunctionNode node)
        {
            if (!OccurenceTable.ContainsKey(node.Name))
                return false;

            var visitedNodes = OccurenceTable[node.Name];

            for (int i = 0, j = visitedNodes.Count; i < j; ++i)
                if (AreNodesEqaul(node, visitedNodes[i]))
                    return true;

            return false;
        }

        /// <summary>
        ///     Determine if two function invokations can be measured as equal.
        /// </summary>
        /// <param name="node">The first node.</param>
        /// <param name="functionNode">The second node.</param>
        /// <returns>True if invokations are considered as equal, else false.</returns>
        private bool AreNodesEqaul(RawFunctionNode node, RawFunctionNode functionNode)
        {
            var areNodesEqual = node.Name == functionNode.Name && node.ReturnType == functionNode.ReturnType;

            if (node.Descendants.Length != functionNode.Descendants.Length)
                return false;

            var j = node.Descendants.Length;
            for (var i = 0; i < j && areNodesEqual; ++i)
            {
                if (node.Descendants[i].ReturnType != functionNode.Descendants[i].ReturnType)
                    return false;

                if (node.Descendants[i].ToString() != functionNode.Descendants[i].ToString())
                    return false;
            }

            return areNodesEqual;
        }
    }
}