using System;
using System.Diagnostics;
using System.Reflection;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class CallExternalNumeric : IRdlInstruction
    {
        private readonly MethodInfo info;
        protected readonly object obj;

        public CallExternalNumeric(object obj, MethodInfo info)
        {
            this.obj = obj;
            this.info = info;
        }

        public void Run(RDLVirtualMachine machine)
        {
            var result = info.Invoke(obj, machine.CallArgs);
            machine.Values.Push(Convert.ToInt64(result));
            machine.InstructionPointer += 1;
        }

        public override string ToString() => string.Format($"CALL {info.Name}");
    }
}
