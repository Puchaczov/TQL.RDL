using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public abstract class InInstruction<T> : IRdlInstruction
        where T : struct
    {
        
        private readonly PopFun _pop;

        /// <summary>
        /// Initialize object.
        /// </summary>
        /// <param name="popFun">Pop function that pops from source stack.</param>
        protected InInstruction(PopFun popFun)
        {
            if (popFun == null) throw new ArgumentNullException(nameof(popFun));
            _pop = popFun;
        }

        /// <summary>
        /// Performs "generic in" comparsion. Get how much arguments, pop reference value, and compare
        /// each poped value from stack with such reference value. If one of them match, then push true, else false. 
        /// This operation push it's result to numeric stack.
        /// </summary>
        /// <param name="machine"></param>
        public void Run(RdlVirtualMachine machine)
        {
            var inArgsCount = machine.Registers[(short)Registers.B];

            var toCompare = _pop(machine);

            var result = false;
            var i = 0;
            for (; i < inArgsCount && !result; ++i)
            {
                var tmpRes = _pop(machine);
                result |= tmpRes.Equals(toCompare);
            }
            for (; i < inArgsCount; ++i)
            {
                _pop(machine);
            }
            machine.Values.Push(result ? 1 : 0);
            
            machine.InstructionPointer += 1;
        }

        /// <summary>
        /// Represents short information about instruction. 
        /// </summary>
        /// <returns>short description of instruction</returns>
        public override string ToString() => "IN-GENERIC";

        /// <summary>
        /// Pop value from stack.
        /// </summary>
        /// <param name="machine">Virtual machine which registers will be used.</param>
        /// <returns>Poped value from stack.</returns>
        protected delegate T PopFun(RdlVirtualMachine machine);
    }
}