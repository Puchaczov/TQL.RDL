using System;
using TQL.RDL.Parser;

namespace TQL.RDL.Evaluator.Attributes
{
    public class InjectPartOfDateTypeAttribute : InjectTypeAttribute
    {
        public override Type InjectType => typeof(PartOfDate);
    }
}
