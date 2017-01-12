using System;
using System.Diagnostics;
using System.Reflection;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class CallExternalDatetime : IRdlInstruction
    {
        protected readonly MethodInfo Info;
        protected object Obj;

        public CallExternalDatetime(object obj, MethodInfo info)
        {
            Obj = obj;
            Info = info;
        }

        public void Run(RdlVirtualMachine machine)
        {
            var result = Info.Invoke(Obj, machine.CallArgs);
            machine.Datetimes.Push((DateTimeOffset)result);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => $"CALL EXTERNAL {Info}";
    }
}