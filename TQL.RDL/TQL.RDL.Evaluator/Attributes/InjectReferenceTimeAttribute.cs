using System;

namespace TQL.RDL.Evaluator.Attributes
{
    public class InjectReferenceTimeAttribute : InjectTypeAttribute
    {
        public override Type InjectType => typeof(DateTimeOffset);
    }
}
