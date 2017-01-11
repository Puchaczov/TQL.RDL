using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public abstract class InInstruction<T> : IRdlInstruction
        where T : struct
    {
        protected delegate T PopFun(RDLVirtualMachine machine);
        protected delegate void PushFun(RDLVirtualMachine machine, T value);
        protected delegate T PeekFun(RDLVirtualMachine machine);

        private readonly PopFun pop;
        private readonly PushFun push;
        private readonly PeekFun peek;

        protected InInstruction(PopFun popFun, PushFun pushFun, PeekFun peekFun)
        {
            pop = popFun;
            push = pushFun;
            peek = peekFun;
        }

        public void Run(RDLVirtualMachine machine)
        {
            var inArgsCount = machine.Registers[(short)Registers.B];

            var toCompare = pop(machine);

            var result = false;
            var i = 0;
            for (; i < inArgsCount && !result; ++i)
            {
                var tmpRes = pop(machine);
                result |= tmpRes.Equals(toCompare);
            }
            for (; i < inArgsCount; ++i)
            {
                pop(machine);
            }
            machine.Values.Push(result ? 1 : 0);
            
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "IN-GENERIC";
    }
}