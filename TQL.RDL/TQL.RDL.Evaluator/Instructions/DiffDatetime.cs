using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class DiffDatetime : IRdlInstruction
    {
        /// <summary>
        /// Determine if two datetime values stored on stack are the same
        /// </summary>
        /// <param name="machine">virtual machine that on that code will be performed</param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Values.Push(Convert.ToInt16(machine.Datetimes.Pop() != machine.Datetimes.Pop()));
            machine.InstructionPointer += 1;
        }
    }
}