using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class PushDateTimeInstruction : IRdlInstruction
    {
        private DateTimeOffset value;

        public PushDateTimeInstruction(DateTimeOffset value)
        {
            this.value = value;
        }

        public void Run(RDLVirtualMachine machine)
        {
            machine.Datetimes.Push(value);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => string.Format("PUSH {0}", value);
    }
}