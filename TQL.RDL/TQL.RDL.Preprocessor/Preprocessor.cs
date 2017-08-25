using System;
using TQL.Common.Filters;
using TQL.Common.Filters.Pipeline;

namespace TQL.RDL.Preprocessor
{
    public class Preprocessor : Pipeline<string>
    {
        private DateTimeOffset _time;
        private string[] _formats;

        /// <summary>
        /// Instantiate object.
        /// </summary>
        /// <param name="time">The time.</param>
        public Preprocessor(DateTimeOffset time, string[] formats)
        {
            _time = time;
            _formats = formats;
            RegisterFilters();
        }

        private void RegisterFilters()
        {
            Register(new Trim());
            Register(new ReplaceStartAtWithTime(_time, _formats, Strings.StartAtDateExtractRegex));
        }
    }
}