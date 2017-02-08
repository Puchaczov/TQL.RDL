using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class LessEqualDatetime : IRdlInstruction
    {
        /// <summary>
        /// Performs comparsion betwen two datetime values in stack and determine if it's less or equal. 
        /// </summary>
        /// <param name="machine">The virtual machine.</param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Values.Push(machine.Datetimes.Pop() >= machine.Datetimes.Pop() ? 1 : 0);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "LESSEQUAL-DATETIME";
    }
}