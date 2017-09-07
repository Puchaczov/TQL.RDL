using System;

namespace TQL.RDL.Evaluator.Attributes
{
    public sealed class InjectStopAtAttribute : InjectTypeAttribute
    {
        public override Type InjectType => typeof(DateTimeOffset?);
    }
}
