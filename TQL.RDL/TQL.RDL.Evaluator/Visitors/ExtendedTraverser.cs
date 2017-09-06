using System.Collections.Generic;
using TQL.RDL.Evaluator.Scope;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Visitors
{
    public class ExtendedTraverser : Traverser
    {   
        private readonly ScopeContext _scope;

        /// <summary>
        /// Initialize instance.
        /// </summary>
        /// <param name="codeGenerationVisitor">The destination visitor.</param>
        /// <param name="methodOccurences">Method occurences dictionary.</param>
        /// <param name="scope">The scope.</param>
        public ExtendedTraverser(INodeVisitor codeGenerationVisitor, IDictionary<string, int> methodOccurences, ScopeContext scope)
            : base(codeGenerationVisitor)
        {
            _scope = scope;
        }

        /// <summary>
        ///     Visit Function node in DFS manner.
        /// </summary>
        /// <param name="node">Function node that will be visited.</param>
        public override void Visit(RawFunctionNode node)
        {
            var scope = _scope.GetScopeByFunction(node);
            var inScopePlace = scope.HasVisibleInvocation(node);
            if(!IsCacheable(node))
            {
                base.Visit(node);
            }
            else if(inScopePlace == Scope.InvocationVisibilityStatus.FirstCall)
            {
                Visit(new CallFunctionAndStoreValueNode(node));
            }
            else
            {
                Visit(new CachedFunctionNode(node));
            }
        }

        /// <summary>
        ///     Determine if function is cacheable.
        /// </summary>
        /// <param name="node">The raw function.</param>
        /// <returns>True if node is cacheable, else false.</returns>
        private bool IsCacheable(RawFunctionNode node)
        {
            return !node.DoNotCache;
        }
    }
}