using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class InInstructionDatetime : InInstruction<DateTimeOffset>
    {
        public InInstructionDatetime()
            : base(x => x.Datetimes.Pop(), (x, v) => x.Datetimes.Push(v), x => x.Datetimes.Peek())
        { }

        public override string ToString() => "IN-DATETIME";
    }
}