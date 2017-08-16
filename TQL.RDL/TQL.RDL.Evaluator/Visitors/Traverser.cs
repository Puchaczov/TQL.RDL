using System;
using System.Collections.Generic;
using System.Linq;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Visitors
{
    public class Traverser : INodeVisitor
    {
        private readonly INodeVisitor _visitor;

        protected readonly Dictionary<string, FunctionOccurenceMetadata> OccurenceTable;

        /// <summary>
        ///     Initialize object.
        /// </summary>
        /// <param name="visitor">Visitor that will generate code for VM</param>
        public Traverser(INodeVisitor visitor)
        {
            if (visitor == null) throw new ArgumentNullException(nameof(visitor));

            _visitor = visitor;
            OccurenceTable = new Dictionary<string, FunctionOccurenceMetadata>();
        }

        /// <summary>
        ///     Visits where node in DFS manner.
        /// </summary>
        /// <param name="node">Where node that will be visited.</param>
        public virtual void Visit(WhereConditionsNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit StopAt node.
        /// </summary>
        /// <param name="node">StopAt node that will be visited.</param>
        public void Visit(StopAtNode node)
        {
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit RepeatEvery node.
        /// </summary>
        /// <param name="node">RepeatEvery node that will be visited.</param>
        public void Visit(RepeatEveryNode node)
        {
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit OrNode node in DFS manner.
        /// </summary>
        /// <param name="node">OrNode node that will be visited.</param>
        public void Visit(OrNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit DateTime node.
        /// </summary>
        /// <param name="node">DateTime node that will be visited.</param>
        public void Visit(DateTimeNode node)
        {
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit Equality node in DFS manner.
        /// </summary>
        /// <param name="node">Equality node that will be visited.</param>
        public void Visit(EqualityNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit ArgList node.
        /// </summary>
        /// <param name="node">ArgList node that will be visited.</param>
        public void Visit(ArgListNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit Function node in DFS manner.
        /// </summary>
        /// <param name="node">Function node that will be visited.</param>
        public virtual void Visit(RawFunctionNode node)
        {
            foreach (var item in node.Descendants.Reverse())
                item.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit CachedFunction.
        /// </summary>
        /// <param name="node">The CachedFunction node.</param>
        public void Visit(CachedFunctionNode node)
        {
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit StoreValueFunction node.
        /// </summary>
        /// <param name="node">StoreValueFunction node of AST.</param>
        public void Visit(CallFunctionAndStoreValueNode node)
        {
            foreach (var item in node.Descendants.Reverse())
                item.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit Numeric node.
        /// </summary>
        /// <param name="node">Numeric node that will be visited.</param>
        public void Visit(NumericNode node)
        {
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit GreaterEqual node in DFS manner.
        /// </summary>
        /// <param name="node">GreaterEqual node that will be visited.</param>
        public void Visit(GreaterEqualNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit LessEqual node in DFS manner.
        /// </summary>
        /// <param name="node">LessEqual node that will be visited.</param>
        public void Visit(LessEqualNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit Add node in DFS manner.
        /// </summary>
        /// <param name="node">Add node that will be visited.</param>
        public void Visit(AddNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit Modulo node.
        /// </summary>
        /// <param name="node">Modulo node that will be visited.</param>
        public void Visit(ModuloNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit FSlashN node in DFS manner.
        /// </summary>
        /// <param name="node">FSlash node that will be visited.</param>
        public void Visit(FSlashNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit Not node in DFS manner.
        /// </summary>
        /// <param name="notNode">Not node that will be visited.</param>
        public void Visit(NotNode notNode)
        {
            notNode.Descendant.Accept(this);
            notNode.Accept(_visitor);
        }

        /// <summary>
        ///     Visit Case node in BFS manner.
        /// </summary>
        /// <param name="node">Case node that will be visited.</param>
        public void Visit(CaseNode node)
        {
            node.Accept(_visitor);
            foreach (var item in node.Descendants)
                item.Accept(this);
        }

        /// <summary>
        ///     Visit WhenThen node in BFS manner.
        /// </summary>
        /// <param name="node">FSlashNode node that will be visited.</param>
        public void Visit(WhenThenNode node)
        {
            node.Accept(_visitor);
            foreach (var item in node.Descendants)
                item.Accept(this);
        }

        /// <summary>
        ///     Visit Else node in BFS manner.
        /// </summary>
        /// <param name="node">Else node that will be visited.</param>
        public virtual void Visit(ElseNode node)
        {
            node.Descendant.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit When node in DFS manner.
        /// </summary>
        /// <param name="node">When node that will be visited.</param>
        public virtual void Visit(WhenNode node)
        {
            node.Descendant.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit Then node in DFS manner.
        /// </summary>
        /// <param name="node">Then node that will be visited.</param>
        public virtual void Visit(ThenNode node)
        {
            node.Descendant.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit Star node in DFS manner.
        /// </summary>
        /// <param name="node">Star node that will be visited.</param>
        public void Visit(StarNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit Hyphen node in DFS manner.
        /// </summary>
        /// <param name="node">Hyphen node that will be visited.</param>
        public void Visit(HyphenNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit NumericConsequentRepeatEvery node in DFS manner.
        /// </summary>
        /// <param name="node">NumericConsequentRepeatEvery node that will be visited.</param>
        public void Visit(NumericConsequentRepeatEveryNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit Less node in DFS manner.
        /// </summary>
        /// <param name="node">Less node that will be visited.</param>
        public void Visit(LessNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit Greater node in DFS manner.
        /// </summary>
        /// <param name="node">Greater node that will be visited.</param>
        public void Visit(GreaterNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit Var node in DFS manner.
        /// </summary>
        /// <param name="node">Var node that will be visited.</param>
        public void Visit(VarNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit NotIn node in DFS manner.
        /// </summary>
        /// <param name="node">NotIn node that will be visited.</param>
        public void Visit(NotInNode node)
        {
            node.Right.Accept(this);
            node.Left.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit Diff node in DFS manner.
        /// </summary>
        /// <param name="node">Diff node that will be visited.</param>
        public void Visit(DiffNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit In node in DFS manner.
        /// </summary>
        /// <param name="node">In node that will be visited.</param>
        public void Visit(InNode node)
        {
            node.Right.Accept(this);
            node.Left.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit And node in DFS manner.
        /// </summary>
        /// <param name="node">And node that will be visited.</param>
        public void Visit(AndNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit RootScript node in DFS manner.
        /// </summary>
        /// <param name="node">RootScript node that will be visited.</param>
        public void Visit(RootScriptNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_visitor);

            OccurenceTable.Clear();
        }

        /// <summary>
        ///     Visit StartAt node.
        /// </summary>
        /// <param name="node">StartAt node that will be visited.</param>
        public void Visit(StartAtNode node)
        {
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit Word node in DFS manner.
        /// </summary>
        /// <param name="node">Word node that will be visited.</param>
        public void Visit(WordNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_visitor);
        }

        /// <summary>
        ///     Visit Between node in DFS manner.
        /// </summary>
        /// <param name="node">Between node that will be visited.</param>
        public void Visit(BetweenNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_visitor);
        }
    }
}