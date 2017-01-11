using System;
using System.Collections.Generic;

namespace TQL.RDL.Evaluator
{
    public class MemoryVariables : Dictionary<string, object>
    {
        public DateTimeOffset Current => (DateTimeOffset)base["current"];
        public int Second => Current.Second;
        public int Minute => Current.Minute;
        public int Hour => Current.Hour;
        public int Day => Current.Day;
        public int Month => Current.Month;
        public int Year => Current.Year;
        public DayOfWeek DayOfWeek => Current.DayOfWeek;
    }
}