using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class DiffNumeric : IRdlInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push((machine.Values.Pop() != machine.Values.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }
    }
}