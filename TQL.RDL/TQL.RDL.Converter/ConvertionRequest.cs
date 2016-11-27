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
        public DateTimeOffset? MinDate { get; }
        public DateTimeOffset? MaxDate { get; }

        public ConvertionRequest(string query, DateTimeOffset? referenceTime = null, bool debuggable = false, DateTimeOffset? minDate = null, DateTimeOffset? maxDate = null, params MethodInfo[] methods)
        {
            ReferenceTime = referenceTime;
            Query = query;
            MethodsToBind = methods;
            Debuggable = debuggable;
            MinDate = minDate;
            MaxDate = maxDate;
        }
    }
}
