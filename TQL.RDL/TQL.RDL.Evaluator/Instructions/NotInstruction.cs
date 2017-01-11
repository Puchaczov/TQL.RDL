using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class NotInstruction : IRdlInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push(Convert.ToInt64(!Convert.ToBoolean(machine.Values.Pop())));
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "NOT";
    }
}