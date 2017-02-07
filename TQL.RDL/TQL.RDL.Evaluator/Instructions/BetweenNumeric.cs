using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TQL.RDL.Evaluator.Instructions
{
    class BetweenNumeric : IRdlInstruction
    {
        public void Run(RdlVirtualMachine machine)
        {
            var max = machine.Values.Pop();
            var min = machine.Values.Pop();
            var val = machine.Values.Pop();

            machine.Values.Push(Convert.ToInt16(val >= min && val < max));
            machine.InstructionPointer += 1;
        }
    }
}
