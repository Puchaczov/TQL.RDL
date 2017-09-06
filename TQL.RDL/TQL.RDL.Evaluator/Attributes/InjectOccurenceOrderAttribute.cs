using System;

namespace TQL.RDL.Evaluator.Attributes
{
    public class InjectOccurenceOrderAttribute : InjectTypeAttribute
    {
        public override Type InjectType => typeof(int);
    }
}
