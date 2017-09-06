using System.Collections.Generic;
using TQL.RDL.Evaluator.Scope;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Visitors
{
    public class ContextGenerator : INodeVisitor
    {
        private readonly ScopeContext _rootScope;
        private readonly Dictionary<RawFunctionNode, ScopeContext> _functionScopeTable;
        private ScopeContext _scope;
        private readonly Stack<bool> _contextChangeTracker;

        public ScopeContext Scope => _scope;

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        /// <param name="contextChangeTracker">Context change tracker.</param>
        public ContextGenerator(Stack<bool> contextChangeTracker)
        {
            _functionScopeTable = new Dictionary<RawFunctionNode, ScopeContext>();
            _rootScope = new ScopeContext(null, _functionScopeTable, "root");
            _scope = _rootScope;
            _contextChangeTracker = contextChangeTracker;
        }

        /// <summary>
        ///     Visits where node in DFS manner.
        /// </summary>
        /// <param name="node">Where node that will be visited.</param>
        public void Visit(WhereConditionsNode node)
        {
            while (_contextChangeTracker.Count > 0)
            {
                _contextChangeTracker.Pop();
                _scope = _scope.OuterScope;
            }

            _scope = new ScopeContext(_scope, _functionScopeTable, "where");
        }

        /// <summary>
        ///     Visit When node in DFS manner.
        /// </summary>
        /// <param name="node">When node that will be visited.</param>
        public void Visit(WhenNode node)
        {
            while (_contextChangeTracker.Count > 0)
            {
                _contextChangeTracker.Pop();
                _scope = _scope.OuterScope;
            }

            _scope = new ScopeContext(_scope, _functionScopeTable, "when");
        }

        /// <summary>
        ///     Visit Then node in DFS manner.
        /// </summary>
        /// <param name="node">Then node that will be visited.</param>
        public void Visit(ThenNode node)
        {
            while (_contextChangeTracker.Count > 0)
            {
                _contextChangeTracker.Pop();
                _scope = _scope.OuterScope;
            }

            _scope = new ScopeContext(_scope, _functionScopeTable, "then");
        }

        /// <summary>
        ///     Visit Else node in BFS manner.
        /// </summary>
        /// <param name="node">Else node that will be visited.</param>
        public void Visit(ElseNode node)
        {
            while (_contextChangeTracker.Count > 0)
            {
                _contextChangeTracker.Pop();
                _scope = _scope.OuterScope;
            }

            _scope = new ScopeContext(_scope, _functionScopeTable, "else");
        }

        public void Visit(RawFunctionNode node)
        {
            _scope.AddFunction(node);
        }

        public void Visit(WordNode node)
        { }

        public void Visit(StartAtNode node)
        { }

        public void Visit(StopAtNode node)
        { }

        public void Visit(RootScriptNode node)
        { }

        public void Visit(RepeatEveryNode node)
        { }

        public void Visit(NotNode notNode)
        { }

        public void Visit(AndNode node)
        { }

        public void Visit(OrNode node)
        { }

        public void Visit(InNode node)
        { }

        public void Visit(DateTimeNode node)
        { }

        public void Visit(DiffNode node)
        { }

        public void Visit(EqualityNode node)
        { }

        public void Visit(NotInNode node)
        { }

        public void Visit(ArgListNode node)
        { }

        public void Visit(VarNode node)
        { }

        public void Visit(NumericNode node)
        { }

        public void Visit(GreaterNode node)
        { }

        public void Visit(GreaterEqualNode node)
        { }

        public void Visit(LessNode node)
        { }

        public void Visit(LessEqualNode node)
        { }

        public void Visit(NumericConsequentRepeatEveryNode node)
        { }

        public void Visit(AddNode node)
        { }

        public void Visit(HyphenNode node)
        { }

        public void Visit(ModuloNode node)
        { }

        public void Visit(StarNode node)
        { }

        public void Visit(FSlashNode node)
        { }

        public void Visit(CaseNode node)
        { }

        public void Visit(WhenThenNode node)
        { }

        public void Visit(BetweenNode node)
        { }

        public void Visit(CachedFunctionNode node)
        {
        }

        public void Visit(CallFunctionAndStoreValueNode node)
        {
        }
    }
}
