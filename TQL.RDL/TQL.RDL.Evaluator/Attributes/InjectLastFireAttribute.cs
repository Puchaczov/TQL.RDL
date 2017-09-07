using System;

namespace TQL.RDL.Evaluator.Attributes
{
    public sealed class InjectLastFireAttribute : InjectTypeAttribute
    {
        public override Type InjectType
            => typeof(DateTimeOffset);
    }
}
