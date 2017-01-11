using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class PushNumericInstruction : IRdlInstruction
    {
        private long value;

        public PushNumericInstruction(long value)
        {
            this.value = value;
        }

        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push(value);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => string.Format("PUSH {0}", value);
    }
}