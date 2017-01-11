using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class MultiplyNumerics : IRdlInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push(machine.Values.Pop() * machine.Values.Pop());
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "MULTIPLY";
    }
}