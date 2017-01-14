using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class GreaterDatetime : IRdlInstruction
    {
        /// <summary>
        /// Determine if one datetime on stack is greater than other.
        /// </summary>
        /// <param name="machine">virtual machine that on that code will be performed</param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Values.Push((machine.Datetimes.Pop() < machine.Datetimes.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }
    }
}