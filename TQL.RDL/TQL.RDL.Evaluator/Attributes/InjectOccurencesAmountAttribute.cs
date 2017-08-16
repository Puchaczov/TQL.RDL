using System;
using System.Collections.Generic;
using System.Text;

namespace TQL.RDL.Evaluator.Attributes
{
    public class InjectOccurencesAmountAttribute : InjectTypeAttribute
    {
        public override Type InjectType => typeof(int);
    }
}
