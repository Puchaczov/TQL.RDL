using System;
using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class InInstructionDatetime : InInstruction<DateTimeOffset>
    {
        /// <summary>
        /// Initialize object.
        /// </summary>
        public InInstructionDatetime()
            : base(x => x.Datetimes.Pop())
        { }

        /// <summary>
        /// Represents short information about instruction.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => "IN-DATETIME";
    }
}