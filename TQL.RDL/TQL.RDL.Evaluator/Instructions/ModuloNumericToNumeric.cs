using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class ModuloNumericToNumeric : IRdlInstruction
    {
        /// <summary>
        /// Performs modulo operation on numeric values.
        /// </summary>
        /// <param name="machine">The virtual machine.</param>
        public void Run(RdlVirtualMachine machine)
        {
            var b = machine.Values.Pop();
            var a = machine.Values.Pop();
            machine.Values.Push(a % b);
            machine.InstructionPointer += 1;
        }

        /// <summary>
        /// Stringify object.
        /// </summary>
        /// <returns>string representation of object.</returns>
        public override string ToString() => "MOD";
    }
}