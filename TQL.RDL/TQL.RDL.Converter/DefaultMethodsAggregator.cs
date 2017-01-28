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

        [BindableMethod]
        public long GetMonth([InjectReferenceTime] DateTimeOffset datetime) => datetime.Month;

        [BindableMethod]
        public long GetYear([InjectReferenceTime] DateTimeOffset datetime) => datetime.Year;

        [BindableMethod]
        public long GetSecond([InjectReferenceTime] DateTimeOffset datetime) => datetime.Second;

        [BindableMethod]
        public long GetMinute([InjectReferenceTime] DateTimeOffset datetime) => datetime.Minute;

        [BindableMethod]
        public long GetHour([InjectReferenceTime] DateTimeOffset datetime) => datetime.Hour;

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

        [BindableMethod]
        public long GetDayOfYear([InjectReferenceTime] DateTimeOffset datetime) => datetime.DayOfYear;

        [BindableMethod]
        public long GetDayOfWeek([InjectReferenceTime] DateTimeOffset datetime) => (long)datetime.DayOfWeek;

        [BindableMethod]
        public bool IsWorkingDay([InjectReferenceTime] DateTimeOffset datetime) => WorkingDays.Contains(datetime.DayOfWeek);
    }
}
