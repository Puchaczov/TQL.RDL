using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using TQL.Core.Converters;

namespace TQL.RDL.Converter
{
    public class ConvertionRequest : ConvertionRequestBase
    {
        /// <summary>
        /// Allow user to instantiate properly configured request. 
        /// </summary>
        /// <param name="query">User defined query.</param>
        /// <param name="source">Source timezone in which query will be evaluated</param>
        /// <param name="target">Target timezone in which evaluated date will be returned to user.</param>
        /// <param name="debuggable">Should query be debuggable?</param>
        /// <param name="formats">Default formats of date in typed query</param>
        /// <param name="methods">C# Methods that will be bind to query</param>
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

        /// <summary>
        /// Query script typed by user.
        /// </summary>
        public string Query { get; }

        /// <summary>
        /// User defined custom methods
        /// </summary>
        public IEnumerable<MethodInfo> MethodsToBind { get; }

        /// <summary>
        /// Determine if query should be debuggable
        /// </summary>
        public bool Debuggable { get; }

        /// <summary>
        /// Determine source timezone in which query will be evaluated
        /// </summary>
        public TimeZoneInfo Source { get; }

        /// <summary>
        /// Determine target timezone in which evaluated date will be returned to user.
        /// </summary>
        public TimeZoneInfo Target { get; }

        /// <summary>
        /// Define avaliable date formats which user can put in quries.
        /// </summary>
        public string[] Formats { get; }

        /// <summary>
        /// Determine culture type of typed date (currently, always en-US)
        /// </summary>
        public CultureInfo CultureInfo { get; }
    }
}
