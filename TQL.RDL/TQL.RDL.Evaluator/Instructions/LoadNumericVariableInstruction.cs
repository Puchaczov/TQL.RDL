using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class LoadNumericVariableInstruction : IRdlInstruction
    {
        private readonly Func<MemoryVariables, int> _loadFun;

        public LoadNumericVariableInstruction(Func<MemoryVariables, int> func)
        {
            _loadFun = func;
        }

        public void Run(RdlVirtualMachine machine)
        {
            machine.Values.Push(_loadFun(machine.Variables));
            machine.InstructionPointer += 1;
        }
    }
}