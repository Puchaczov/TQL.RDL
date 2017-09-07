using System;

namespace TQL.RDL.Evaluator.Attributes
{
    public sealed class InjectStartAtAttribute : InjectTypeAttribute
    {
        public override Type InjectType => typeof(DateTimeOffset);
    }
}
