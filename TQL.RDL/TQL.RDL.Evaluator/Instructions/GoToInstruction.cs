using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class GoToInstruction : IRdlInstruction
    {
        private readonly int _newInstructionPtr;

        /// <summary>
        /// Initialize object.
        /// </summary>
        /// <param name="newInstructionPtr">Value that determine next instruction that will be performed.</param>
        public GoToInstruction(int newInstructionPtr)
        {
            _newInstructionPtr = newInstructionPtr;
        }

        /// <summary>
        /// Sets new instruction pointer value.
        /// </summary>
        /// <param name="machine">virtual machine that on that code will be performed</param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.InstructionPointer = _newInstructionPtr;
        }

        /// <summary>
        /// Gets instruction short description
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"GOTO { _newInstructionPtr }";
    }
}