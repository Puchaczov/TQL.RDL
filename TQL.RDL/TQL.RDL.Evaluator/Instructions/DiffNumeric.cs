using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class DiffNumeric : IRdlInstruction
    {
        /// <summary>
        /// Determine if two values on stack are different
        /// </summary>
        /// <param name="machine">virtual machine that on that code will be performed</param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Values.Push((machine.Values.Pop() != machine.Values.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }
    }
}