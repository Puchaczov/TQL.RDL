using System;

namespace TQL.RDL.Evaluator.Attributes
{
    public sealed class InjectOccurencesAmountAttribute : InjectTypeAttribute
    {
        public override Type InjectType => typeof(int);
    }
}
