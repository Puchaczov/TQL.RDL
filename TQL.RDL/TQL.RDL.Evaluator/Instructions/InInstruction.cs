using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public abstract class InInstruction<T> : IRdlInstruction
        where T : struct
    {
        protected delegate T PopFun(RdlVirtualMachine machine);
        protected delegate void PushFun(RdlVirtualMachine machine, T value);
        protected delegate T PeekFun(RdlVirtualMachine machine);

        private readonly PopFun _pop;
        private readonly PushFun _push;
        private readonly PeekFun _peek;

        protected InInstruction(PopFun popFun, PushFun pushFun, PeekFun peekFun)
        {
            _pop = popFun;
            _push = pushFun;
            _peek = peekFun;
        }

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

        public override string ToString() => "IN-GENERIC";
    }
}