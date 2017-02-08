using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class Modify : IRdlInstruction
    {
        public delegate DateTimeOffset Fun(DateTimeOffset value);

        private readonly Fun _fun;
        
        /// <summary>
        /// Initialize instance.
        /// </summary>
        /// <param name="fun">Function to modify reference time.</param>
        public Modify(Fun fun)
        {
            _fun = fun;
        }

        /// <summary>
        /// Performs modyfication on reference time.
        /// </summary>
        /// <param name="machine">The virtual machine.</param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.ReferenceTime = _fun(machine.ReferenceTime);
            machine.InstructionPointer += 1;
        }

        /// <summary>
        /// Stringify object.
        /// </summary>
        /// <returns>string representation of object.</returns>
        public override string ToString() => "MODIFY";
    }
}