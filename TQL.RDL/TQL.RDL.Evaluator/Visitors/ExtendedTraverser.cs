using System.Collections.Generic;
using TQL.RDL.Parser.Helpers;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Visitors
{
    public class ExtendedTraverser : Traverser
    {
        /// <summary>
        /// This is very simple function scope implementation.
        /// It allows to track if variable occur in outer scope.
        /// </summary>
        private class Scope
        {
            private readonly List<string> _functions;

            public Scope OuterScope { get; }

            public Scope(Scope outerScope)
            {
                OuterScope = outerScope;
                _functions = new List<string>();
            }

            /// <summary>
            /// Adds the function to current scope.
            /// </summary>
            /// <param name="node">The function to add.</param>
            public void AddFunction(RawFunctionNode node)
            {
                _functions.Add(node.DetailedFunctionIdentifier());
            }

            /// <summary>
            /// Determine if specific function occured in any of the outer scope.
            /// </summary>
            /// <param name="function">The function to check.</param>
            /// <returns>True if outer scope contains function, otherwise false.</returns>
            public bool HasVisibleInvocation(RawFunctionNode function)
            {
                var identifier = function.DetailedFunctionIdentifier();
                var stack = new Stack<Scope>();

                if(OuterScope != null)
                    stack.Push(OuterScope);

                while (stack.Count > 0)
                {
                    var scope = stack.Pop();

                    if (scope.HasFunctionIdentifier(identifier))
                        return true;

                    if (scope.OuterScope != null)
                        stack.Push(scope.OuterScope);
                }

                return false;
            }

            /// <summary>
            /// Determine if current scope contains function identifier.
            /// </summary>
            /// <param name="identifier"></param>
            /// <returns>true if current scope has identifier, otherwise false.</returns>
            private bool HasFunctionIdentifier(string identifier)
            {
                return _functions.Contains(identifier);
            }
        }
        
        private Scope _scope;

        /// <summary>
        /// Initialize instance.
        /// </summary>
        /// <param name="codeGenerationVisitor">The destination visitor.</param>
        /// <param name="methodOccurences">Method occurences dictionary.</param>
        public ExtendedTraverser(INodeVisitor codeGenerationVisitor, IDictionary<string, int> methodOccurences)
            : base(codeGenerationVisitor)
        {
            _scope = new Scope(null);
        }
        
        /// <summary>
        ///     Visits where node in DFS manner.
        /// </summary>
        /// <param name="node">Where node that will be visited.</param>
        public override void Visit(WhereConditionsNode node)
        {
            var oldScope = _scope;
            _scope = new Scope(_scope);
            base.Visit(node);
            _scope = oldScope;
        }

        /// <summary>
        ///     Visit When node in DFS manner.
        /// </summary>
        /// <param name="node">When node that will be visited.</param>
        public override void Visit(WhenNode node)
        {
            var oldScope = _scope;
            _scope = new Scope(_scope);
            base.Visit(node);
            _scope = oldScope;
        }

        /// <summary>
        ///     Visit Then node in DFS manner.
        /// </summary>
        /// <param name="node">Then node that will be visited.</param>
        public override void Visit(ThenNode node)
        {
            var oldScope = _scope;
            _scope = new Scope(_scope);
            base.Visit(node);
            _scope = oldScope;
        }

        /// <summary>
        ///     Visit Else node in BFS manner.
        /// </summary>
        /// <param name="node">Else node that will be visited.</param>
        public override void Visit(ElseNode node)
        {
            var oldScope = _scope;
            _scope = new Scope(_scope);
            base.Visit(node);
            _scope = oldScope;
        }

        /// <summary>
        ///     Visit Function node in DFS manner.
        /// </summary>
        /// <param name="node">Function node that will be visited.</param>
        public override void Visit(RawFunctionNode node)
        {
            _scope.AddFunction(node);
            if (!CanBeRetrievedFromCache(node) || !IsCacheable(node))
            {
                Visit(new CallFunctionAndStoreValueNode(node));
            }
            else
                Visit(new CachedFunctionNode(node));
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
        ///     Determine if function node were processed earlier and if can be restored from cache.
        /// </summary>
        /// <param name="node">The Function node.</param>
        /// <returns>True if function can be restored from cache, else false.</returns>
        private bool CanBeRetrievedFromCache(RawFunctionNode node)
        {
            return _scope.HasVisibleInvocation(node);
        }
    }
}