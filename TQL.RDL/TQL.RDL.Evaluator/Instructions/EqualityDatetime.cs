using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class EqualityDatetime : IRdlInstruction
    {
        public void Run(RdlVirtualMachine machine)
        {
            machine.Values.Push((machine.Datetimes.Pop() == machine.Datetimes.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }
    }
}