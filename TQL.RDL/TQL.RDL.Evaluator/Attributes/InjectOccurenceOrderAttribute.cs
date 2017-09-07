using System;

namespace TQL.RDL.Evaluator.Attributes
{
    public sealed class InjectOccurenceOrderAttribute : InjectTypeAttribute
    {
        public override Type InjectType => typeof(int);
    }
}
