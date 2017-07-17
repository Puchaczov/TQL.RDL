using System;

namespace TQL.RDL.Evaluator.Attributes
{
    public class InjectLastFireAttribute : InjectTypeAttribute
    {
        public override Type InjectType
            => typeof(DateTimeOffset);
    }
}
