using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class LoadDateTimeVariableInstruction : IRdlInstruction
    {
        private Func<MemoryVariables, DateTimeOffset> loadFun;

        public LoadDateTimeVariableInstruction(Func<MemoryVariables, DateTimeOffset> func)
        {
            this.loadFun = func;
        }

        public void Run(RDLVirtualMachine machine)
        {
            machine.Datetimes.Push(loadFun(machine.Variables));
            machine.InstructionPointer += 1;
        }
    }
}