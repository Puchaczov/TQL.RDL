using System;
using System.Collections.Generic;

namespace TQL.RDL.Evaluator
{
    public class MemoryVariables : Dictionary<string, object>
    {
        /// <summary>
        /// Current variable. (DateTimeOffset)
        /// </summary>
        public DateTimeOffset Current => (DateTimeOffset) base["current"];

        /// <summary>
        /// Second variable. (int)
        /// </summary>
        public int Second => Current.Second;

        /// <summary>
        /// Minute variable. (int)
        /// </summary>
        public int Minute => Current.Minute;

        /// <summary>
        /// Hour variable. (int)
        /// </summary>
        public int Hour => Current.Hour;

        /// <summary>
        /// Day variable. (int)
        /// </summary>
        public int Day => Current.Day;

        /// <summary>
        /// Month variable. (int)
        /// </summary>
        public int Month => Current.Month;

        /// <summary>
        /// Year variable. (int)
        /// </summary>
        public int Year => Current.Year;

        /// <summary>
        /// DayOfWeek variable. (DayOfWeek)
        /// </summary>
        public DayOfWeek DayOfWeek => Current.DayOfWeek;
    }
}