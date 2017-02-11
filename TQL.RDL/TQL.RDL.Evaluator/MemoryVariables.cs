using System;
using System.Collections.Generic;

namespace TQL.RDL.Evaluator
{
    public class MemoryVariables : Dictionary<string, object>
    {
        /// <summary>
        /// Current variable. (DateTimeOffset)
        /// </summary>
        public DateTimeOffset ReferenceTime { get; set; }

        /// <summary>
        /// Second variable. (int)
        /// </summary>
        public int Second => ReferenceTime.Second;

        /// <summary>
        /// Minute variable. (int)
        /// </summary>
        public int Minute => ReferenceTime.Minute;

        /// <summary>
        /// Hour variable. (int)
        /// </summary>
        public int Hour => ReferenceTime.Hour;

        /// <summary>
        /// Day variable. (int)
        /// </summary>
        public int Day => ReferenceTime.Day;

        /// <summary>
        /// Month variable. (int)
        /// </summary>
        public int Month => ReferenceTime.Month;

        /// <summary>
        /// Year variable. (int)
        /// </summary>
        public int Year => ReferenceTime.Year;

        /// <summary>
        /// DayOfWeek variable. (DayOfWeek)
        /// </summary>
        public DayOfWeek DayOfWeek => ReferenceTime.DayOfWeek;
    }
}