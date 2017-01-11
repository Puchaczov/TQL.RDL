using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class Modify : IRdlInstruction
    {
        public delegate void Fun(RDLVirtualMachine machine);

        private readonly Fun fun;

        public Modify(Fun fun)
        {
            this.fun = fun;
        }

        public void Run(RDLVirtualMachine machine)
        {
            fun(machine);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "MODIFY";
    }
}