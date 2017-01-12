using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class PushDateTimeInstruction : IRdlInstruction
    {
        private readonly DateTimeOffset _value;

        public PushDateTimeInstruction(DateTimeOffset value)
        {
            _value = value;
        }

        public void Run(RdlVirtualMachine machine)
        {
            machine.Datetimes.Push(_value);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => string.Format("PUSH {0}", _value);
    }
}