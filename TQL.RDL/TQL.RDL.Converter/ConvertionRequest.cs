using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TQL.Core.Converters;

namespace TQL.RDL.Converter
{
    public class ConvertionRequest : ConvertionRequestBase
    {
        public DateTimeOffset ReferenceTime { get; }
        public string Query { get; }
    }
}
