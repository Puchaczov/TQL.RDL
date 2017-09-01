using System.Collections.Generic;
using TQL.RDL.Parser.Helpers;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Scope
{
    public class ScopeContext
    {
        private readonly List<string> _functions;
        private readonly List<ScopeContext> _innerScopes;
        private readonly string _scopeName;
        private readonly Dictionary<RawFunctionNode, ScopeContext> _functionScopeTable;

        /// <summary>
        /// Gets the outer scope.
        /// </summary>
        public ScopeContext OuterScope { get; }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        /// <param name="outerScope">Outer scope</param>
        /// <param name="name">Easy to debug name.</param>
        public ScopeContext(ScopeContext outerScope, string name)
            : this(outerScope, new Dictionary<RawFunctionNode, ScopeContext>(), name)
        { }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        /// <param name="outerScope">Outer scope</param>
        /// <param name="functionScopeTable">Function scope table.</param>
        /// <param name="name">Easy to debug name.</param>
        public ScopeContext(ScopeContext outerScope, Dictionary<RawFunctionNode, ScopeContext> functionScopeTable, string name)
        {
            OuterScope = outerScope;
            _functions = new List<string>();
            _innerScopes = new List<ScopeContext>();
            _functionScopeTable = functionScopeTable;
            _scopeName = name;

            if (outerScope != null)
                outerScope.AddInnerScope(this);
        }

        /// <summary>
        /// Adds the function to current scope.
        /// </summary>
        /// <param name="node">The function to add.</param>
        public void AddFunction(RawFunctionNode node)
        {
            _functionScopeTable.Add(node, this);
            _functions.Add(node.DetailedFunctionIdentifier());
        }

        public ScopeContext GetScopeByFunction(RawFunctionNode node)
        {
            return _functionScopeTable[node];
        }

        /// <summary>
        /// Adds the inner scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        private void AddInnerScope(ScopeContext scope)
        {
            _innerScopes.Add(scope);
        }

        /// <summary>
        /// Determine if specific function occured in any of the outer scope.
        /// </summary>
        /// <param name="function">The function to check.</param>
        /// <returns>True if outer scope contains function, otherwise false.</returns>
        public InvocationVisibilityStatus HasVisibleInvocation(RawFunctionNode function)
        {
            var identifier = function.DetailedFunctionIdentifier();
            var stack = new Stack<ScopeContext>();

            if (OuterScope != null)
                stack.Push(OuterScope);

            while (stack.Count > 0)
            {
                var scope = stack.Pop();

                if (scope.HasFunctionIdentifier(identifier))
                {
                    return InvocationVisibilityStatus.AnotherCall;
                }

                if (scope.OuterScope != null)
                    stack.Push(scope.OuterScope);
            }

            return InvocationVisibilityStatus.FirstCall;
        }

        /// <summary>
        /// Gets childs scopes of this scope.
        /// </summary>
        /// <returns>Child scopes.</returns>
        private IReadOnlyCollection<ScopeContext> GetChilds()
        {
            return _innerScopes;
        }

        /// <summary>
        /// Gets the root of all scopes.
        /// </summary>
        /// <returns>The root scope.</returns>
        public ScopeContext GetRootOfAllScopes()
        {
            var scope = this;
            while (scope.OuterScope != null)
                scope = scope.OuterScope;

            return scope;
        }

        /// <summary>
        /// Gets the root scope.
        /// </summary>
        /// <returns>The root scope.</returns>
        private ScopeContext GetRootScope()
        {
            var scope = this;
            var pscope = this;
            while (scope.OuterScope != null)
            {
                pscope = scope;
                scope = scope.OuterScope;
            }

            return pscope;
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
}
