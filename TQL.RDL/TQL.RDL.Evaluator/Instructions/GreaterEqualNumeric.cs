using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class GreaterEqualNumeric : IRdlInstruction
    {
        /// <summary>
        /// Determine if two values on stack are greter or equal
        /// </summary>
        /// <param name="machine"></param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Values.Push(machine.Values.Pop() <= machine.Values.Pop() ? 1 : 0);
            machine.InstructionPointer += 1;
        }
    }
}