using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    class RestoreValue : IRdlInstruction
    {
        private readonly Action<RdlVirtualMachine, object> _restoreValueFunc;
        private readonly MethodInfo _registeredFunction;

        /// <summary>
        /// Initialize instance.
        /// </summary>
        /// <param name="registeredFunction">Method that invokation result have to be restored.</param>
        /// <param name="restoreValueFunc">Function that copy result from memory.</param>
        public RestoreValue(MethodInfo registeredFunction, Action<RdlVirtualMachine, object> restoreValueFunc)
        {
            _registeredFunction = registeredFunction;
            _restoreValueFunc = restoreValueFunc;
        }

        /// <summary>
        /// Copies invocation result from memory.
        /// </summary>
        /// <param name="machine">The virtual machine.</param>
        public void Run(RdlVirtualMachine machine)
        {
            _restoreValueFunc(machine, machine.Variables[_registeredFunction.ToString()]);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => $"RESTORE {_registeredFunction}";
    }
}
