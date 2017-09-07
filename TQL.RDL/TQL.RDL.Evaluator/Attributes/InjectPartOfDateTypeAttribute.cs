using System;
using TQL.RDL.Parser;

namespace TQL.RDL.Evaluator.Attributes
{
    public sealed class InjectPartOfDateTypeAttribute : InjectTypeAttribute
    {
        public override Type InjectType => typeof(PartOfDate);
    }
}
