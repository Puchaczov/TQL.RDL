using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class PushNumericInstruction : IRdlInstruction
    {
        private readonly long _value;

        /// <summary>
        ///     Initialize object.
        /// </summary>
        /// <param name="value">Value that have to be pushed onto the stack.</param>
        public PushNumericInstruction(long value)
        {
            _value = value;
        }

        /// <summary>
        ///     Push value onto the stack.
        /// </summary>
        /// <param name="machine">The virtual machine.</param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Values.Push(_value);
            machine.InstructionPointer += 1;
        }

        /// <summary>
        ///     Stringify object.
        /// </summary>
        /// <returns>string representation of object.</returns>
        public override string ToString() => $"PUSH {_value}";
    }
}