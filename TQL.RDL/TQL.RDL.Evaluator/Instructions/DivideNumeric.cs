using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class DivideNumeric : IRdlInstruction
    {
        /// <summary>
        ///     Divide two numeric values stored on stack.
        /// </summary>
        /// <param name="machine">virtual machine that on that code will be performed</param>
        public void Run(RdlVirtualMachine machine)
        {
            var b = machine.Values.Pop();
            var a = machine.Values.Pop();
            machine.Values.Push(a / b);
            machine.InstructionPointer += 1;
        }

        /// <summary>
        ///     Gets instruction short description
        /// </summary>
        /// <returns></returns>
        public override string ToString() => "DIVIDE";
    }
}