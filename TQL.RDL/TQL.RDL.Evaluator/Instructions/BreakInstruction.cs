using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class BreakInstruction : IRdlInstruction
    {
        /// <summary>
        ///     Breaks virtual code execution
        /// </summary>
        /// <param name="machine"></param>
        public void Run(RdlVirtualMachine machine)
        {
            machine.Break = true;
            machine.InstructionPointer += 1;
        }

        /// <summary>
        ///     Gets instruction short description
        /// </summary>
        /// <returns>Stringified object.</returns>
        public override string ToString() => "BREAK";
    }
}