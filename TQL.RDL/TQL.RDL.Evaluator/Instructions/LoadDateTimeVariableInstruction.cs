using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class LoadDateTimeVariableInstruction : IRdlInstruction
    {
        private readonly Func<MemoryVariables, DateTimeOffset> _loadFun;

        public LoadDateTimeVariableInstruction(Func<MemoryVariables, DateTimeOffset> func)
        {
            _loadFun = func;
        }

        public void Run(RdlVirtualMachine machine)
        {
            machine.Datetimes.Push(_loadFun(machine.Variables));
            machine.InstructionPointer += 1;
        }
    }
}