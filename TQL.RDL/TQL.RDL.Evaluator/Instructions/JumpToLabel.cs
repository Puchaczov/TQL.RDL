using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class JumpToLabel : IRdlInstruction
    {
        private readonly string _label;

        /// <summary>
        ///     Initialize object.
        /// </summary>
        /// <param name="label">Define place to jump.</param>
        public JumpToLabel(string label)
        {
            _label = label;
        }

        /// <summary>
        ///     Assign instruction pointer based on label.
        /// </summary>
        /// <param name="machine">Virtual machine on that code will be executed.</param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.InstructionPointer = machine.RelativeLabels[_label];
        }

        /// <summary>
        ///     Stringify object.
        /// </summary>
        /// <returns>string representation of object.</returns>
        public override string ToString() => $"JMP {_label}";
    }
}