using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class Modify : IRdlInstruction
    {
        public delegate void Fun(RdlVirtualMachine machine);

        private readonly Fun _fun;

        public Modify(Fun fun)
        {
            _fun = fun;
        }

        public void Run(RdlVirtualMachine machine)
        {
            _fun(machine);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "MODIFY";
    }
}