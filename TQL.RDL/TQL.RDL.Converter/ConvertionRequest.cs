using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using TQL.Core.Converters;

namespace TQL.RDL.Converter
{
    public class ConvertionRequest : ConvertionRequestBase
    {
        public string Query { get; }
        public IEnumerable<MethodInfo> MethodsToBind { get; }
        public bool Debuggable { get; }
        public TimeZoneInfo Source { get; }
        public TimeZoneInfo Target { get; }
        public string[] Formats { get; }
        public CultureInfo CultureInfo { get; }

        public ConvertionRequest(string query, TimeZoneInfo source, TimeZoneInfo target, bool debuggable = false, string[] formats = null, params MethodInfo[] methods)
        {
            Query = query;
            MethodsToBind = methods;
            Debuggable = debuggable;
            Source = source;
            Target = target;
            Formats = formats;
            CultureInfo = new CultureInfo("en-US");
        }
    }
}
