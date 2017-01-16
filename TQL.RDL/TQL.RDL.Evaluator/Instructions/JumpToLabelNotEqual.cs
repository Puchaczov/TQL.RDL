using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class JumpToLabelNotEqual : IRdlInstruction
    {
        private readonly string _label;

        /// <summary>
        /// Initialize object.
        /// </summary>
        /// <param name="label">Define place to jump.</param>
        public JumpToLabelNotEqual(string label)
        {
            _label = label;
        }

        /// <summary>
        /// Assign instruction pointer based on label when poped from stack value is false, 
        /// else increment instruction pointer.
        /// </summary>
        /// <param name="machine">Virtual machine on that code will be executed.</param>
        public void Run(RdlVirtualMachine machine)
        {
            if (!Convert.ToBoolean(machine.Values.Pop()))
                machine.InstructionPointer = machine.RelativeLabels[_label];
            else
                machine.InstructionPointer += 1;
        }

        /// <summary>
        /// Stringify object.
        /// </summary>
        /// <returns>string representation of object.</returns>
        public override string ToString() => $"JMPNE {_label}";
    }
}