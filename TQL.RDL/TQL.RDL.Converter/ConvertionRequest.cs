using System;
using System.Collections.Generic;
using System.Reflection;
using TQL.Core.Converters;

namespace TQL.RDL.Converter
{
    public class ConvertionRequest : ConvertionRequestBase
    {
        public DateTimeOffset? ReferenceTime { get; }
        public string Query { get; }
        public IEnumerable<MethodInfo> MethodsToBind { get; }
        public bool Debuggable { get; }

        public ConvertionRequest(string query, DateTimeOffset? referenceTime = null, bool debuggable = false, params MethodInfo[] methods)
        {
            ReferenceTime = referenceTime;
            Query = query;
            MethodsToBind = methods;
            Debuggable = debuggable;
        }
    }
}
