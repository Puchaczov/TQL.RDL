using System;

namespace TQL.RDL.Evaluator.Attributes
{
    public sealed class InjectReferenceTimeAttribute : InjectTypeAttribute
    {
        public override Type InjectType => typeof(DateTimeOffset);
    }
}