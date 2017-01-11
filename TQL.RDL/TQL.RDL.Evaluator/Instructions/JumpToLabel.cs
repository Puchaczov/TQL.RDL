using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class JumpToLabel : IRdlInstruction
    {
        private string label;

        public JumpToLabel(string label)
        {
            this.label = label;
        }

        public void Run(RDLVirtualMachine machine)
        {
            machine.InstructionPointer = machine.RelativeLabels[label];
        }

        public override string ToString() => $"JMP {label}";
    }
}