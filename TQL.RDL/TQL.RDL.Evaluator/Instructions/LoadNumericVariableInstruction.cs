using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class LoadNumericVariableInstruction : IRdlInstruction
    {
        private Func<MemoryVariables, int> loadFun;

        public LoadNumericVariableInstruction(Func<MemoryVariables, int> func)
        {
            this.loadFun = func;
        }

        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push(loadFun(machine.Variables));
            machine.InstructionPointer += 1;
        }

    }
}