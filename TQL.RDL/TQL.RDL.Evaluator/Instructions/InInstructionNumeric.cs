using System.Diagnostics;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class InInstructionNumeric : InInstruction<long>
    {
        public InInstructionNumeric()
            : base(x => x.Values.Pop(), (x, v) => x.Values.Push(v), x => x.Values.Peek())
        { }

        public override string ToString() => "IN-NUMERIC";
    }
}