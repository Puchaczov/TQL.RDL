using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class LessDatetime : IRdlInstruction
    {
        /// <summary>
        ///     Performs comparsion betwen two numeric values in stack and determine if it's less
        /// </summary>
        /// <param name="machine">The virtual machine</param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Values.Push(machine.Datetimes.Pop() > machine.Datetimes.Pop() ? 1 : 0);
            machine.InstructionPointer += 1;
        }

        /// <summary>
        ///     Stringify object.
        /// </summary>
        /// <returns>string representation of object.</returns>
        public override string ToString() => "LESS-DATETIME";
    }
}