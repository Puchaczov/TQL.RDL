using System;
using System.Collections.Generic;
using System.Reflection;
using TQL.Core.Converters;

namespace TQL.RDL.Converter
{
    public class ConvertionRequest : ConvertionRequestBase
    {
        public DateTimeOffset ReferenceTime { get; }
        public string Query { get; }
        public IEnumerable<KeyValuePair<object, MethodInfo>> MethodsToBind { get; }

        public ConvertionRequest(DateTimeOffset referenceTime, string query, params KeyValuePair<object, MethodInfo>[] methods)
        {
            ReferenceTime = referenceTime;
            Query = query;
            MethodsToBind = methods;
        }
    }
}
