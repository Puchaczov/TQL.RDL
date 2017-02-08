using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TQL.RDL.Evaluator.Attributes;
using TQL.RDL.Evaluator.Helpers;

namespace TQL.RDL.Converter
{
    [BindableClass]
    public class DefaultMethodsAggregator
    {
        private static readonly DayOfWeek[] WorkingDays = {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday
        };

        /// <summary>
        /// Determine if injected date is last day of month.
        /// </summary>
        /// <param name="datetime">The date.</param>
        /// <returns>true if it's last day of month, else false.</returns>
        [BindableMethod]
        public static bool IsLastDayOfMonth([InjectReferenceTime] DateTimeOffset datetime) => DateTime.DaysInMonth(datetime.Year, datetime.Month) == datetime.Day;

        /// <summary>
        /// Determine if injected date is specific day of week.
        /// </summary>
        /// <param name="datetime">The date.</param>
        /// <param name="dayOfWeek">The day of week.</param>
        /// <returns></returns>
        [BindableMethod]
        public static bool IsDayOfWeek([InjectReferenceTime] DateTimeOffset datetime, long dayOfWeek) => datetime.DayOfWeek == (DayOfWeek)dayOfWeek;

        /// <summary>
        /// Determine if passed number is even.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>True if passed number is even, else false.</returns>
        [BindableMethod]
        public static bool IsEven(long number) => number % 2 == 0;

        /// <summary>
        /// Determine if passed number is odd.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>True if passed number is odd, else false.</returns>
        [BindableMethod]
        public static bool IsOdd(long number) => number % 2 != 0;
        
        /// <summary>
        /// Gets current time.
        /// </summary>
        /// <returns>Current time.</returns>
        [BindableMethod]
        public static DateTimeOffset Now() => DateTimeOffset.Now;

        /// <summary>
        /// Gets current time in UTC.
        /// </summary>
        /// <returns>Current time (UTC).</returns>
        [BindableMethod]
        public static DateTimeOffset UtcNow() => DateTimeOffset.UtcNow;

        /// <summary>
        /// Gets the day of month.
        /// </summary>
        /// <param name="datetime">The datetime</param>
        /// <returns>Day of month.</returns>
        [BindableMethod]
        public long GetDay([InjectReferenceTime] DateTimeOffset datetime) => datetime.Day;

        /// <summary>
        /// Gets month of year.
        /// </summary>
        /// <param name="datetime">The datetime</param>
        /// <returns>Month of year.</returns>
        [BindableMethod]
        public long GetMonth([InjectReferenceTime] DateTimeOffset datetime) => datetime.Month;

        /// <summary>
        /// Gets year.
        /// </summary>
        /// <param name="datetime">The datetime</param>
        /// <returns>Year</returns>
        [BindableMethod]
        public long GetYear([InjectReferenceTime] DateTimeOffset datetime) => datetime.Year;

        /// <summary>
        /// Gets second.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns>Second</returns>
        [BindableMethod]
        public long GetSecond([InjectReferenceTime] DateTimeOffset datetime) => datetime.Second;

        /// <summary>
        /// Gets minute.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        [BindableMethod]
        public long GetMinute([InjectReferenceTime] DateTimeOffset datetime) => datetime.Minute;

        /// <summary>
        /// Gets hour.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns>Hour.</returns>
        [BindableMethod]
        public long GetHour([InjectReferenceTime] DateTimeOffset datetime) => datetime.Hour;

        /// <summary>
        /// Gets week of month.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <param name="type">Type of calcuation to get value.</param>
        /// <returns>Week of month.</returns>
        [BindableMethod]
        public long GetWeekOfMonth([InjectReferenceTime] DateTimeOffset datetime, string type = "")
        {
            switch (type)
            {
                case "simple":
                    break;
                case "calculated":
                    return datetime.WeekOfMonth(DayOfWeek.Sunday);
                case "iso":
                    break;
            }
            return datetime.WeekOfMonth();
        }

        /// <summary>
        /// Gets day of year.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns>Day of year.</returns>
        [BindableMethod]
        public long GetDayOfYear([InjectReferenceTime] DateTimeOffset datetime) => datetime.DayOfYear;

        /// <summary>
        /// Gets day of week.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns>Day of week.</returns>
        [BindableMethod]
        public long GetDayOfWeek([InjectReferenceTime] DateTimeOffset datetime) => (long)datetime.DayOfWeek;

        /// <summary>
        /// Determine if the date is working day.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns>True if it's working day, else false.</returns>
        [BindableMethod]
        public bool IsWorkingDay([InjectReferenceTime] DateTimeOffset datetime) => WorkingDays.Contains(datetime.DayOfWeek);
    }
}
