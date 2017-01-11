using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class JumpToLabelNotEqual : IRdlInstruction
    {
        private readonly string label;

        public JumpToLabelNotEqual(string label)
        {
            this.label = label;
        }

        public void Run(RDLVirtualMachine machine)
        {
            if (!Convert.ToBoolean(machine.Values.Pop()))
                machine.InstructionPointer = machine.RelativeLabels[label];
            else
                machine.InstructionPointer += 1;
        }

        public override string ToString() => $"JMPNE {label}";
    }
}