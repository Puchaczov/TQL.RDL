using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class JumpToLabel : IRdlInstruction
    {
        private readonly string _label;

        public JumpToLabel(string label)
        {
            _label = label;
        }

        public void Run(RdlVirtualMachine machine)
        {
            machine.InstructionPointer = machine.RelativeLabels[_label];
        }

        public override string ToString() => $"JMP {_label}";
    }
}