using System.Diagnostics;
using RDL.Parser.Nodes;
using TQL.RDL.Evaluator.Visitors;

namespace TQL.RDL.Evaluator.Enumerators
{
    [DebuggerDisplay("{Node.ToString(),nq}")]
    public class VisitationState
    {
        /// <summary>
        /// Initialize object
        /// </summary>
        /// <param name="node">Node that has to be tracked</param>
        public VisitationState(RdlSyntaxNode node)
        {
            Node = node;
            ToVisitDescendantIndex = 0;
            WasVisitedOnce = false;
        }

        /// <summary>
        /// Call Accept on node and track it's visited
        /// </summary>
        /// <param name="analyzer">Perform analyzis on node</param>
        public void Accept(AnalyzerBase analyzer)
        {
            Node.Accept(analyzer);
            WasVisitedOnce = true;
        }

        #region Public properties

        /// <summary>
        /// Node that is tracked.
        /// </summary>
        public RdlSyntaxNode Node { get; }

        /// <summary>
        /// Determine which of node descendants were already visited.
        /// </summary>
        public int ToVisitDescendantIndex { get; set; }

        /// <summary>
        /// Determine if that node were visited
        /// </summary>
        public bool WasVisitedOnce { get; set; }

        #endregion
    }
}
