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
    class CopyAndStoreValue<TTypeToStore> : IRdlInstruction
    {
        private readonly Func<RdlVirtualMachine, TTypeToStore> _storeValueFunc;
        private readonly MethodInfo _registeredFunction;

        /// <summary>
        /// Initialize instance.
        /// </summary>
        /// <param name="registeredFunction">Method that invokation result have to be stored.</param>
        /// <param name="storeValueFunc">Function that copy result from virtual machine.</param>
        public CopyAndStoreValue(MethodInfo registeredFunction, Func<RdlVirtualMachine, TTypeToStore> storeValueFunc)
        {
            _registeredFunction = registeredFunction;
            _storeValueFunc = storeValueFunc;
        }

        /// <summary>
        /// Copies invocation result to memory.
        /// </summary>
        /// <param name="machine">The virtual machine.</param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Variables[_registeredFunction.ToString()] = _storeValueFunc(machine);
            machine.InstructionPointer += 1;
        }

        /// <summary>
        /// Gets instruction short description
        /// </summary>
        /// <returns>Stringified object.</returns>
        public override string ToString() => $"COPY-STORE {_registeredFunction}";
    }
}
