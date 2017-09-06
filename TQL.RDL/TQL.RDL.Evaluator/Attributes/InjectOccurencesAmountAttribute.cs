using System;

namespace TQL.RDL.Evaluator.Attributes
{
    public class InjectOccurencesAmountAttribute : InjectTypeAttribute
    {
        public override Type InjectType => typeof(int);
    }
}
