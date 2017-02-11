using System;

namespace TQL.RDL.Evaluator.Instructions
{
    class BetweenDatetime : IRdlInstruction
    {
        /// <summary>
        ///     Performs check if the value is between other values.
        /// </summary>
        /// <param name="machine">The virtual machine.</param>
        public void Run(RdlVirtualMachine machine)
        {
            var max = machine.Datetimes.Pop();
            var min = machine.Datetimes.Pop();
            var val = machine.Datetimes.Pop();

            machine.Values.Push(Convert.ToInt16(val >= min && val < max));
            machine.InstructionPointer += 1;
        }
    }
}