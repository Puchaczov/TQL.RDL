using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class InInstructionNumeric : InInstruction<long>
    {
        /// <summary>
        ///     Initialize object.
        /// </summary>
        public InInstructionNumeric()
            : base(x => x.Values.Pop())
        {
        }

        /// <summary>
        ///     Stringify object.
        /// </summary>
        /// <returns>string representation of object.</returns>
        public override string ToString() => "IN-NUMERIC";
    }
}