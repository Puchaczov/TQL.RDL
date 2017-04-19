using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TQL.RDL.Evaluator.Attributes
{
    public class InjectLastFireAttribute : InjectTypeAttribute
    {
        public override Type InjectType
            => typeof(DateTimeOffset);
    }
}
