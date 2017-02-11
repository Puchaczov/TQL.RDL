using System;

namespace TQL.RDL.Evaluator.Helpers
{
    public static class DateTimeOffsetHelper
    {
        /// <summary>
        ///     Get week of month by simply check
        ///     if day of month is between multiplicy of seven and seven days after that date
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns>Week of month</returns>
        public static int WeekOfMonth(this DateTimeOffset datetime)
        {
            var day = datetime.Day;

            if (day <= 7)
                return 1;
            if (day <= 14)
                return 2;
            if (day <= 21)
                return 3;
            if (day <= 28)
                return 4;
            return 5;
        }

        /// <summary>
        ///     Get week of month by check
        ///     if day of month is between some date and seven days after that date.
        ///     It take care of considered DayOfWeek that starts week and
        ///     take care of some months doesn't start from considered DayOfWeek
        /// </summary>
        /// <param name="datetime">The DateTimeOffset.</param>
        /// <param name="startDayOfWeek">The DayOfWeek.</param>
        /// <returns>Week of month</returns>
        public static int WeekOfMonth(this DateTimeOffset datetime, DayOfWeek startDayOfWeek)
        {
            var day = datetime.Day;
            var weekDay = datetime.DayOfWeek;
            var firstDayOfMonth = new DateTimeOffset(datetime.Year, datetime.Month, 1, 0, 0, 0, datetime.Offset);
            var operationalDay = firstDayOfMonth;

            var whichWeek = 0;
            if (operationalDay.DayOfWeek != startDayOfWeek)
            {
                whichWeek = 1;
                while (operationalDay.DayOfWeek != startDayOfWeek)
                    operationalDay = operationalDay.AddDays(1);
            }

            if (day >= operationalDay.Day && day < operationalDay.Day + 7)
                return whichWeek + 1;
            if (day >= operationalDay.Day + 7 && day < operationalDay.Day + 14)
                return whichWeek + 2;
            if (day >= operationalDay.Day + 14 && day < operationalDay.Day + 21)
                return whichWeek + 3;
            if (day >= operationalDay.Day + 21 && day < operationalDay.Day + 28)
                return whichWeek + 4;
            return whichWeek;
        }
    }
}