using System;
using System.Globalization;
using TQL.Core.Converters;

namespace TQL.RDL
{
    public class ConvertionRequest<TMethodsAggregator> : ConvertionRequestBase where TMethodsAggregator : new()
    {
        /// <summary>
        ///     Allow user to instantiate properly configured request.
        /// </summary>
        /// <param name="query">User defined query.</param>
        /// <param name="source">Source timezone in which query will be evaluated</param>
        /// <param name="target">Target timezone in which evaluated date will be returned to user.</param>
        /// <param name="debuggable">Should query be debuggable?</param>
        /// <param name="formats">Default formats of date in typed query</param>
        public ConvertionRequest(string query, TimeZoneInfo source, TimeZoneInfo target, bool debuggable = false,
            string[] formats = null)
        {
            Query = query;
            MethodsAggregator = new TMethodsAggregator();
            Debuggable = debuggable;
            Source = source;
            Target = target;
            Formats = formats;
            CultureInfo = new CultureInfo("en-US");
        }

        /// <summary>
        ///     Query script typed by user.
        /// </summary>
        public string Query { get; }

        /// <summary>
        ///     User defined custom methods
        /// </summary>
        public TMethodsAggregator MethodsAggregator { get; }

        /// <summary>
        ///     Determine if query should be debuggable
        /// </summary>
        public bool Debuggable { get; }

        /// <summary>
        ///     Determine source timezone in which query will be evaluated
        /// </summary>
        public TimeZoneInfo Source { get; }

        /// <summary>
        ///     Determine target timezone in which evaluated date will be returned to user.
        /// </summary>
        public TimeZoneInfo Target { get; }

        /// <summary>
        ///     Define avaliable date formats which user can put in quries.
        /// </summary>
        public string[] Formats { get; }

        /// <summary>
        ///     Determine culture type of typed date (currently, always en-US)
        /// </summary>
        public CultureInfo CultureInfo { get; }
    }
}