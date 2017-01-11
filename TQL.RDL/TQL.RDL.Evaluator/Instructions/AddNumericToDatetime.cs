using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class AddNumericToDatetime : IRdlInstruction
    {
        public AddNumericToDatetime()
        { }

        public void Run(RDLVirtualMachine machine)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => "ADD";
    }
}