using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class AndInstruction : IRdlInstruction
    {
        public void Run(RdlVirtualMachine machine)
        {
            var b = machine.Values.Pop();
            var a = machine.Values.Pop();
            machine.Values.Push(a & b);
            machine.InstructionPointer += 1;
        }
    }
}