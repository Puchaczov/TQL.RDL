using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class PushStringInstruction : IRdlInstruction
    {
        private readonly string _value;

        /// <summary>
        ///     Initialize object.
        /// </summary>
        /// <param name="value">String that have to be pushed.</param>
        public PushStringInstruction(string value)
        {
            _value = value;
        }

        /// <summary>
        ///     Push value onto the stack.
        /// </summary>
        /// <param name="machine">The virtual machine.</param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Strings.Push(_value);
            machine.InstructionPointer += 1;
        }

        /// <summary>
        ///     Stringify object.
        /// </summary>
        /// <returns>string representation of object.</returns>
        public override string ToString() => $"PUSH STRING '{_value}'";
    }
}