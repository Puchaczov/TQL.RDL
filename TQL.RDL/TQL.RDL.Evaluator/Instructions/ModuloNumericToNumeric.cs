using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class ModuloNumericToNumeric : IRdlInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            var b = machine.Values.Pop();
            var a = machine.Values.Pop();
            machine.Values.Push(a % b);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "MOD";
    }
}