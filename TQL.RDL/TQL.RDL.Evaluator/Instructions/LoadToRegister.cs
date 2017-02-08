using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class LoadToRegister : IRdlInstruction
    {
        private readonly Registers _register;
        private readonly long _value;

        /// <summary>
        /// Initialize instance.
        /// </summary>
        /// <param name="reg">Register that will hold the value.</param>
        /// <param name="value"></param>
        public LoadToRegister(Registers reg, long value)
        {
            _value = value;
            _register = reg;
        }

        /// <summary>
        /// Assign value to one of registers.
        /// </summary>
        /// <param name="machine">The virtual machine.</param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Registers[(short)_register] = _value;
            machine.InstructionPointer += 1;
        }

        /// <summary>
        /// Stringify object.
        /// </summary>
        /// <returns>string representation of object.</returns>
        public override string ToString() => $"LDReg {_register}, {_value}";
    }
}