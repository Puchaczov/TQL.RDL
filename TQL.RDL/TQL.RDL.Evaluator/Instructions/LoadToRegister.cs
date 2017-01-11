using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class LoadToRegister : IRdlInstruction
    {
        private readonly long value;
        private readonly Registers register;

        public LoadToRegister(Registers reg, long value)
        {
            this.value = value;
            this.register = reg;
        }

        public void Run(RDLVirtualMachine machine)
        {
            machine.Registers[(short)register] = value;
            machine.InstructionPointer += 1;
        }

        public override string ToString() => $"LDReg {register}, {value}";
    }
}