using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class OrInstruction : IRdlInstruction
    {
        /// <summary>
        ///     Performs "OR" operation.
        /// </summary>
        /// <param name="machine">The virtual machine.</param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Values.Push(machine.Values.Pop() | machine.Values.Pop());
            machine.InstructionPointer += 1;
        }
    }
}