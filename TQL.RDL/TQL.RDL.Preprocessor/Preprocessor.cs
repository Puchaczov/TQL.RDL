using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TQL.Common.Filters;
using TQL.Common.Pipeline;

namespace TQL.RDL
{
    public class Preprocessor : Pipeline<string>
    {
        public Preprocessor()
        {
            base.Register(new Trim());
        }
    }
}
