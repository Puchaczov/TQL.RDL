using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class PushDateTimeInstruction : IRdlInstruction
    {
        private readonly DateTimeOffset _value;

        /// <summary>
        ///     Initialize instance.
        /// </summary>
        /// <param name="value">Value to be pushed on stack.</param>
        public PushDateTimeInstruction(DateTimeOffset value)
        {
            _value = value;
        }

        /// <summary>
        ///     Push passed value onto the stack.
        /// </summary>
        /// <param name="machine"></param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Datetimes.Push(_value);
            machine.InstructionPointer += 1;
        }

        /// <summary>
        ///     Stringify object.
        /// </summary>
        /// <returns>string representation of object.</returns>
        public override string ToString() => $"PUSH {_value}";
    }
}