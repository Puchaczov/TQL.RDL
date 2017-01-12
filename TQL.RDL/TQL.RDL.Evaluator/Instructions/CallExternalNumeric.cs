using System;
using System.Diagnostics;
using System.Reflection;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class CallExternalNumeric : IRdlInstruction
    {
        private readonly MethodInfo _info;
        protected readonly object Obj;

        public CallExternalNumeric(object obj, MethodInfo info)
        {
            Obj = obj;
            _info = info;
        }

        public void Run(RdlVirtualMachine machine)
        {
            var result = _info.Invoke(Obj, machine.CallArgs);
            machine.Values.Push(Convert.ToInt64(result));
            machine.InstructionPointer += 1;
        }

        public override string ToString() => string.Format($"CALL {_info.Name}");
    }
}
