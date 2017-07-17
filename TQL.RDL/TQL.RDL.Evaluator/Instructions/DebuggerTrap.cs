using System;
using System.Diagnostics;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class DebuggerTrap : IRdlInstruction
    {
        public delegate void OnExpressionBegin(RdlSyntaxNode node, IVmTracker tracker);

        public delegate void OnExpressionEnd(RdlSyntaxNode node, IVmTracker tracker);

        public enum ExpressionState
        {
            Before,
            After
        }

        private readonly OnExpressionBegin _expBeginCallback;
        private readonly OnExpressionEnd _expEndCallback;

        private readonly RdlSyntaxNode _node;
        private readonly ExpressionState _state;

        public DebuggerTrap(RdlSyntaxNode node, ExpressionState state, OnExpressionBegin expBeginCallback,
            OnExpressionEnd expEndCallback)
        {
            _node = node;
            _state = state;
            _expBeginCallback = expBeginCallback;
            _expEndCallback = expEndCallback;
        }

        public void Run(RdlVirtualMachine machine)
        {
            switch (_state)
            {
                case ExpressionState.Before:
                    _expBeginCallback(_node, machine);
                    break;
                case ExpressionState.After:
                    _expEndCallback(_node, machine);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}