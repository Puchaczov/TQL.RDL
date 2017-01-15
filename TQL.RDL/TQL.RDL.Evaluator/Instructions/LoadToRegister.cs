using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class LoadToRegister : IRdlInstruction
    {
        private readonly Registers _register;
        private readonly long _value;

        public LoadToRegister(Registers reg, long value)
        {
            _value = value;
            _register = reg;
        }

        public void Run(RdlVirtualMachine machine)
        {
            machine.Registers[(short)_register] = _value;
            machine.InstructionPointer += 1;
        }

        public override string ToString() => $"LDReg {_register}, {_value}";
    }
}