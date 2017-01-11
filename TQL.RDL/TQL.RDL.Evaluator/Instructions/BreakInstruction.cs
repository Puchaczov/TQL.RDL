using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class BreakInstruction : IRdlInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Break = true;
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "BREAK";
    }
}