using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Visitors
{
    public class BfsTraverser : INodeVisitor
    {
        protected readonly INodeVisitor _visitor;

        /// <summary>
        ///     Initialize object.
        /// </summary>
        /// <param name="visitor">Visitor that will generate code for VM</param>
        public BfsTraverser(INodeVisitor visitor)
        {
            _visitor = visitor ?? throw new ArgumentNullException(nameof(visitor));
        }

        /// <summary>
        ///     Visits where node in BFS manner.
        /// </summary>
        /// <param name="node">Where node that will be visited.</param>
        public virtual void Visit(WhereConditionsNode node)
        {
            node.Accept(_visitor);
            foreach (var item in node.Descendants)
                item.Accept(this);
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
        ///     Visit OrNode node in BFS manner.
        /// </summary>
        /// <param name="node">OrNode node that will be visited.</param>
        public void Visit(OrNode node)
        {
            node.Accept(_visitor);
            node.Left.Accept(this);
            node.Right.Accept(this);
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
        ///     Visit Equality node in BFS manner.
        /// </summary>
        /// <param name="node">Equality node that will be visited.</param>
        public void Visit(EqualityNode node)
        {
            node.Accept(_visitor);
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        /// <summary>
        ///     Visit ArgList node.
        /// </summary>
        /// <param name="node">ArgList node that will be visited.</param>
        public void Visit(ArgListNode node)
        {
            node.Accept(_visitor);
            foreach (var item in node.Descendants)
                item.Accept(this);
        }

        /// <summary>
        ///     Visit Function node in BFS manner.
        /// </summary>
        /// <param name="node">Function node that will be visited.</param>
        public virtual void Visit(RawFunctionNode node)
        {
            node.Accept(_visitor);
            foreach (var item in node.Descendants.Reverse())
                item.Accept(this);
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
            node.Accept(_visitor);
            foreach (var item in node.Descendants.Reverse())
                item.Accept(this);
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
        ///     Visit GreaterEqual node in BFS manner.
        /// </summary>
        /// <param name="node">GreaterEqual node that will be visited.</param>
        public void Visit(GreaterEqualNode node)
        {
            node.Accept(_visitor);
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        /// <summary>
        ///     Visit LessEqual node in BFS manner.
        /// </summary>
        /// <param name="node">LessEqual node that will be visited.</param>
        public void Visit(LessEqualNode node)
        {
            node.Accept(_visitor);
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        /// <summary>
        ///     Visit Add node in BFS manner.
        /// </summary>
        /// <param name="node">Add node that will be visited.</param>
        public void Visit(AddNode node)
        {
            node.Accept(_visitor);
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        /// <summary>
        ///     Visit Modulo node.
        /// </summary>
        /// <param name="node">Modulo node that will be visited.</param>
        public void Visit(ModuloNode node)
        {
            node.Accept(_visitor);
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        /// <summary>
        ///     Visit FSlashN node in BFS manner.
        /// </summary>
        /// <param name="node">FSlash node that will be visited.</param>
        public void Visit(FSlashNode node)
        {
            node.Accept(_visitor);
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        /// <summary>
        ///     Visit Not node in BFS manner.
        /// </summary>
        /// <param name="notNode">Not node that will be visited.</param>
        public void Visit(NotNode notNode)
        {
            notNode.Accept(_visitor);
            notNode.Descendant.Accept(this);
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
            node.Accept(_visitor);
            node.Descendant.Accept(this);
        }

        /// <summary>
        ///     Visit When node in BFS manner.
        /// </summary>
        /// <param name="node">When node that will be visited.</param>
        public virtual void Visit(WhenNode node)
        {
            node.Accept(_visitor);
            node.Descendant.Accept(this);
        }

        /// <summary>
        ///     Visit Then node in BFS manner.
        /// </summary>
        /// <param name="node">Then node that will be visited.</param>
        public virtual void Visit(ThenNode node)
        {
            node.Accept(_visitor);
            node.Descendant.Accept(this);
        }

        /// <summary>
        ///     Visit Star node in BFS manner.
        /// </summary>
        /// <param name="node">Star node that will be visited.</param>
        public void Visit(StarNode node)
        {
            node.Accept(_visitor);
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        /// <summary>
        ///     Visit Hyphen node in BFS manner.
        /// </summary>
        /// <param name="node">Hyphen node that will be visited.</param>
        public void Visit(HyphenNode node)
        {
            node.Accept(_visitor);
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        /// <summary>
        ///     Visit NumericConsequentRepeatEvery node in BFS manner.
        /// </summary>
        /// <param name="node">NumericConsequentRepeatEvery node that will be visited.</param>
        public void Visit(NumericConsequentRepeatEveryNode node)
        {
            node.Accept(_visitor);
            foreach (var item in node.Descendants)
                item.Accept(this);
        }

        /// <summary>
        ///     Visit Less node in BFS manner.
        /// </summary>
        /// <param name="node">Less node that will be visited.</param>
        public void Visit(LessNode node)
        {
            node.Accept(_visitor);
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        /// <summary>
        ///     Visit Greater node in BFS manner.
        /// </summary>
        /// <param name="node">Greater node that will be visited.</param>
        public void Visit(GreaterNode node)
        {
            node.Accept(_visitor);
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        /// <summary>
        ///     Visit Var node in BFS manner.
        /// </summary>
        /// <param name="node">Var node that will be visited.</param>
        public void Visit(VarNode node)
        {
            node.Accept(_visitor);
            foreach (var item in node.Descendants)
                item.Accept(this);
        }

        /// <summary>
        ///     Visit NotIn node in BFS manner.
        /// </summary>
        /// <param name="node">NotIn node that will be visited.</param>
        public void Visit(NotInNode node)
        {
            node.Accept(_visitor);
            node.Right.Accept(this);
            node.Left.Accept(this);
        }

        /// <summary>
        ///     Visit Diff node in BFS manner.
        /// </summary>
        /// <param name="node">Diff node that will be visited.</param>
        public void Visit(DiffNode node)
        {
            node.Accept(_visitor);
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        /// <summary>
        ///     Visit In node in BFS manner.
        /// </summary>
        /// <param name="node">In node that will be visited.</param>
        public void Visit(InNode node)
        {
            node.Accept(_visitor);
            node.Right.Accept(this);
            node.Left.Accept(this);
        }

        /// <summary>
        ///     Visit And node in BFS manner.
        /// </summary>
        /// <param name="node">And node that will be visited.</param>
        public void Visit(AndNode node)
        {
            node.Accept(_visitor);
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        /// <summary>
        ///     Visit RootScript node in BFS manner.
        /// </summary>
        /// <param name="node">RootScript node that will be visited.</param>
        public void Visit(RootScriptNode node)
        {
            node.Accept(_visitor);
            foreach (var item in node.Descendants)
                item.Accept(this);
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
        ///     Visit Word node in BFS manner.
        /// </summary>
        /// <param name="node">Word node that will be visited.</param>
        public void Visit(WordNode node)
        {
            node.Accept(_visitor);
            foreach (var item in node.Descendants)
                item.Accept(this);
        }

        /// <summary>
        ///     Visit Between node in BFS manner.
        /// </summary>
        /// <param name="node">Between node that will be visited.</param>
        public void Visit(BetweenNode node)
        {
            node.Accept(_visitor);
            foreach (var item in node.Descendants)
                item.Accept(this);
        }
    }
}
