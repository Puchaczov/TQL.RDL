using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class GoToInstruction : IRdlInstruction
    {
        private readonly int _newInstructionPtr;

        public GoToInstruction(int newInstructionPtr)
        {
            _newInstructionPtr = newInstructionPtr;
        }

        public void Run(RdlVirtualMachine machine)
        {
            machine.InstructionPointer = _newInstructionPtr;
        }

        public override string ToString() => $"GOTO { _newInstructionPtr }";
    }
}