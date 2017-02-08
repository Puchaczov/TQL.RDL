using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class NotInstruction : IRdlInstruction
    {
        /// <summary>
        /// Perform negate operation on value.
        /// </summary>
        /// <param name="machine">The virtual machine.</param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Values.Push(Convert.ToInt64(!Convert.ToBoolean(machine.Values.Pop())));
            machine.InstructionPointer += 1;
        }

        /// <summary>
        /// Stringify object.
        /// </summary>
        /// <returns>string representation of object.</returns>
        public override string ToString() => "NOT";
    }
}