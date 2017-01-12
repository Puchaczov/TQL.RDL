using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class JumpToLabelNotEqual : IRdlInstruction
    {
        private readonly string _label;

        public JumpToLabelNotEqual(string label)
        {
            _label = label;
        }

        public void Run(RdlVirtualMachine machine)
        {
            if (!Convert.ToBoolean(machine.Values.Pop()))
                machine.InstructionPointer = machine.RelativeLabels[_label];
            else
                machine.InstructionPointer += 1;
        }

        public override string ToString() => $"JMPNE {_label}";
    }
}