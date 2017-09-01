using System;
using System.Collections.Generic;
using System.Text;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Visitors
{
    public class ContextGeneratorTraverser : BfsTraverser
    {
        private readonly Stack<bool> _contextChangeTracker;

        public ContextGeneratorTraverser(INodeVisitor visitor, Stack<bool> contextChangeTracker) : base(visitor)
        {
            _contextChangeTracker = contextChangeTracker;
        }

        /// <summary>
        ///     Visits where node in BFS manner.
        /// </summary>
        /// <param name="node">Where node that will be visited.</param>
        public override void Visit(WhereConditionsNode node)
        {
            node.Accept(_visitor);
            foreach (var item in node.Descendants)
                item.Accept(this);
            _contextChangeTracker.Push(true);
        }

        /// <summary>
        ///     Visit When node in BFS manner.
        /// </summary>
        /// <param name="node">When node that will be visited.</param>
        public override void Visit(WhenNode node)
        {
            node.Accept(_visitor);
            node.Descendant.Accept(this);
            _contextChangeTracker.Push(true);
        }
        
        /// <summary>
        ///     Visit Then node in BFS manner.
        /// </summary>
        /// <param name="node">Then node that will be visited.</param>
        public override void Visit(ThenNode node)
        {
            node.Accept(_visitor);
            node.Descendant.Accept(this);
            _contextChangeTracker.Push(true);
        }

        /// <summary>
        ///     Visit Else node in BFS manner.
        /// </summary>
        /// <param name="node">Else node that will be visited.</param>
        public override void Visit(ElseNode node)
        {
            node.Accept(_visitor);
            node.Descendant.Accept(this);
            _contextChangeTracker.Push(true);
        }
    }
}
