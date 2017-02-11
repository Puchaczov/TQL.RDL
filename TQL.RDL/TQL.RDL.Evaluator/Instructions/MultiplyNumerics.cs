using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class MultiplyNumerics : IRdlInstruction
    {
        /// <summary>
        ///     Multiply two numeric values.
        /// </summary>
        /// <param name="machine">The virtual machine.</param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Values.Push(machine.Values.Pop() * machine.Values.Pop());
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "MULTIPLY";
    }
}