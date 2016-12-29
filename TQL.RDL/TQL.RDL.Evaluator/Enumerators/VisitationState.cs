using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Enumerators
{
    [DebuggerDisplay("{Node.ToString(),nq}")]
    public class VisitationState
    {
        public RdlSyntaxNode Node { get; }
        public int ToVisitDescendantIndex { get; set; }
        public bool WasVisitedOnce { get; set; }

        public VisitationState(RdlSyntaxNode node)
        {
            this.Node = node;
            ToVisitDescendantIndex = 0;
            WasVisitedOnce = false;
        }

        public void Accept(AnalyzerBase analyzer)
        {
            Node.Accept(analyzer);
            WasVisitedOnce = true;
        }
    }
}
