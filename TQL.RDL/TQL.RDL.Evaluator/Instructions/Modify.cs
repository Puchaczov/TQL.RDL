using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class Modify : IRdlInstruction
    {
        public delegate DateTimeOffset Fun(DateTimeOffset value);

        private readonly Fun _fun;

        public Modify(Fun fun)
        {
            _fun = fun;
        }

        public void Run(RdlVirtualMachine machine)
        {
            machine.ReferenceTime = _fun(machine.ReferenceTime);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "MODIFY";
    }
}