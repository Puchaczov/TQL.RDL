using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TQL.Common.Filters;
using TQL.Common.Pipeline;

namespace TQL.RDL.Preprocessor
{
    public class Preprocessor : Pipeline<string>
    {
        public Preprocessor()
        {
            Register(new Trim());
        }
    }
}
