using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TQL.RDL.Evaluator.Instructions
{
    class BetweenDatetime : IRdlInstruction
    {
        public void Run(RdlVirtualMachine machine)
        {
            var max = machine.Datetimes.Pop();
            var min = machine.Datetimes.Pop();
            var val = machine.Datetimes.Pop();

            machine.Values.Push(Convert.ToInt16(val >= min && val < max));
            machine.InstructionPointer += 1;
        }
    }
}
