using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class PushNumericInstruction : IRdlInstruction
    {
        private readonly long _value;

        public PushNumericInstruction(long value)
        {
            _value = value;
        }

        public void Run(RdlVirtualMachine machine)
        {
            machine.Values.Push(_value);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => string.Format("PUSH {0}", _value);
    }
}