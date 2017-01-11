using System.Diagnostics;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class DebuggerTrap : IRdlInstruction
    {
        public enum ExpressionState
        {
            Before,
            After
        }

        private RdlSyntaxNode node;
        private ExpressionState state;
        private OnExpressionBegin expBeginCallback;
        private OnExpressionEnd expEndCallback;

        public delegate void OnExpressionBegin(RdlSyntaxNode node, IVmTracker tracker);
        public delegate void OnExpressionEnd(RdlSyntaxNode node, IVmTracker tracker);

        public DebuggerTrap(RdlSyntaxNode node, ExpressionState state, OnExpressionBegin expBeginCallback, OnExpressionEnd expEndCallback)
        {
            this.node = node;
            this.state = state;
            this.expBeginCallback = expBeginCallback;
            this.expEndCallback = expEndCallback;
        }

        public void Run(RDLVirtualMachine machine)
        {
            switch(state)
            {
                case ExpressionState.Before:
                    expBeginCallback(node, machine);
                    break;
                case ExpressionState.After:
                    expEndCallback(node, machine);
                    break;
            }
        }
    }
}