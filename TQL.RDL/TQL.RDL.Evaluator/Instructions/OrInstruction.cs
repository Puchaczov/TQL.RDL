using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class OrInstruction : IRdlInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push(machine.Values.Pop() | machine.Values.Pop());
            machine.InstructionPointer += 1;
        }
    }
}