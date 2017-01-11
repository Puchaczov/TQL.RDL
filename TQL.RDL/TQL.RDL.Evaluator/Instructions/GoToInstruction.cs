using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class GoToInstruction : IRdlInstruction
    {
        private readonly int newInstructionPtr;

        public GoToInstruction(int newInstructionPtr)
        {
            this.newInstructionPtr = newInstructionPtr;
        }

        public void Run(RDLVirtualMachine machine)
        {
            machine.InstructionPointer = newInstructionPtr;
        }

        public override string ToString() => $"GOTO { newInstructionPtr }";
    }
}