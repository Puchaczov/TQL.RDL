using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class AddNumericToNumeric : IRdlInstruction
    {
        /// <summary>
        /// Adds two values poped from stack
        /// </summary>
        /// <param name="machine"></param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Values.Push(machine.Values.Pop() + machine.Values.Pop());
            machine.InstructionPointer += 1;
        }

        /// <summary>
        /// Gets instruction short description
        /// </summary>
        /// <returns>Description.</returns>
        public override string ToString() => "ADD";
    }
}