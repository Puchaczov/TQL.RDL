using System;
using System.Diagnostics;
using System.Reflection;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class CallExternalNumeric : IRdlInstruction
    {
        private readonly MethodInfo _info;
        private readonly object _obj;

        /// <summary>
        /// Initialize object
        /// </summary>
        /// <param name="obj">object on that method will be executed</param>
        /// <param name="info">method to execute</param>
        public CallExternalNumeric(object obj, MethodInfo info)
        {
            _obj = obj;
            _info = info;
        }

        /// <summary>
        /// Performs call on object
        /// </summary>
        /// <param name="machine"></param>
        public void Run(RdlVirtualMachine machine)
        {
            var result = _info.Invoke(_obj, machine.CallArgs);
            machine.Values.Push(Convert.ToInt64(result));
            machine.InstructionPointer += 1;
        }

        /// <summary>
        /// Gets instruction short description
        /// </summary>
        /// <returns></returns>
        public override string ToString() => string.Format($"CALL {_info.Name}");
    }
}
