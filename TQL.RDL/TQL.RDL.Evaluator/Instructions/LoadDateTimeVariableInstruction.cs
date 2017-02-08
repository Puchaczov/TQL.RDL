using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class LoadDateTimeVariableInstruction : IRdlInstruction
    {
        private readonly Func<MemoryVariables, DateTimeOffset> _loadFun;

        /// <summary>
        /// Initialize instance.
        /// </summary>
        /// <param name="func">Function to gets datetime variable from memory.</param>
        public LoadDateTimeVariableInstruction(Func<MemoryVariables, DateTimeOffset> func)
        {
            _loadFun = func;
        }

        /// <summary>
        /// Perform stored function to retrieve variable from memory and push it on stack.
        /// </summary>
        /// <param name="machine">The virtual machine.</param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Datetimes.Push(_loadFun(machine.Variables));
            machine.InstructionPointer += 1;
        }
    }
}