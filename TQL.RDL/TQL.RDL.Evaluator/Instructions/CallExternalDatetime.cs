using System;
using System.Diagnostics;
using System.Reflection;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class CallExternalDatetime : IRdlInstruction
    {
        protected readonly MethodInfo info;
        protected object obj;

        public CallExternalDatetime(object obj, MethodInfo info)
        {
            this.obj = obj;
            this.info = info;
        }

        public void Run(RDLVirtualMachine machine)
        {
            var result = info.Invoke(obj, machine.CallArgs);
            machine.Datetimes.Push((DateTimeOffset)result);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => $"CALL EXTERNAL {info}";
    }
}