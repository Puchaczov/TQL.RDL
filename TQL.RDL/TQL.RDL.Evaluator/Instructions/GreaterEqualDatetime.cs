using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class GreaterEqualDatetime : IRdlInstruction
    {
        /// <summary>
        /// Performs comparsion between two datetimes on stack.
        /// </summary>
        /// <param name="machine">Virtual machine on that code will be performed</param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Values.Push(machine.Datetimes.Pop() <= machine.Datetimes.Pop() ? 1 : 0);
            machine.InstructionPointer += 1;
        }
    }
}