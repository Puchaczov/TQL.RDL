using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class LessEqualNumeric : IRdlInstruction
    {
        public void Run(RdlVirtualMachine machine)
        {
            machine.Values.Push((machine.Values.Pop() >= machine.Values.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "LESSEQUAL";
    }
}