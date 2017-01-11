using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class GreaterEqualDatetime : IRdlInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push((machine.Datetimes.Pop() <= machine.Datetimes.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }
    }
}