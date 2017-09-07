using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TQL.RDL.Evaluator.Attributes;
using TQL.RDL.Evaluator.Helpers;
using TQL.RDL.Parser;

namespace TQL.RDL
{
    [BindableClass]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class DefaultMethodsAggregator
    {
        private static readonly DayOfWeek[] WorkingDays =
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday
        };

        private readonly Random _random = new Random();
        private readonly int _instanceRandomNumber;

        private readonly List<int> _randomizedValues = new List<int>();
        private int _lastPartOfDayValue = -1;

        /// <summary>
        /// Initialize instance.
        /// </summary>
        public DefaultMethodsAggregator()
        {
            _instanceRandomNumber = _random.Next();
        }

        /// <summary>
        ///     Determine if last fire occured at least N [part of datetime] ago.
        /// </summary>
        /// <param name="referenceTime">Injected reference time.</param>
        /// <param name="lastFire">Injected last fire.</param>
        /// <param name="value">todo: describe value parameter on EveryNth</param>
        /// <param name="partOfDate">todo: describe partOfDate parameter on EveryNth</param>
        /// <returns>True if last fire occured N [part of datetime] ago or it hasn't last fire set yet, otherwise false.</returns>
        [BindableMethod]
        public static bool EveryNth([InjectReferenceTime] DateTimeOffset referenceTime, [InjectLastFire] DateTimeOffset? lastFire, int value, string partOfDate)
        {
            if (!lastFire.HasValue)
                return true;

            var diff = referenceTime - lastFire.Value;
            switch (partOfDate)
            {
                case "second":
                    return diff.TotalSeconds >= value;
                case "minute":
                    return diff.TotalMinutes >= value;
                case "hour":
                    return diff.TotalHours >= value;
                case "day":
                    return diff.TotalDays >= value;
            }

            throw new NotSupportedException(partOfDate);
        }

        /// <summary>
        ///     Determine if injected date is last day of month.
        /// </summary>
        /// <param name="datetime">The date.</param>
        /// <returns>true if it's last day of month, else false.</returns>
        [BindableMethod]
        public static bool IsLastDayOfMonth([InjectReferenceTime] DateTimeOffset datetime)
            => DateTime.DaysInMonth(datetime.Year, datetime.Month) == datetime.Day;

        /// <summary>
        ///     Determine if injected date is specific day of week.
        /// </summary>
        /// <param name="datetime">The date.</param>
        /// <param name="dayOfWeek">The day of week.</param>
        /// <returns>True when reference DayOfWeek is equals to passed dayOfWeek.</returns>
        [BindableMethod]
        public static bool IsDayOfWeek([InjectReferenceTime] DateTimeOffset datetime, long dayOfWeek)
            => datetime.DayOfWeek == (DayOfWeek) dayOfWeek;

        /// <summary>
        /// Gets the day of week for timeline year and month.
        /// </summary>
        /// <param name="datetime">Reference time.</param>
        /// <param name="day">Day of month.</param>
        /// <returns>Calculated DayOfWeek.</returns>
        [BindableMethod]
        public static int GetDayOfWeek([InjectReferenceTime] DateTimeOffset datetime, int day)
        {
            return (int)new DateTimeOffset(datetime.Year, datetime.Month, day, datetime.Hour, datetime.Minute, datetime.Second, datetime.Offset).DayOfWeek;
        }

        /// <summary>
        ///     Determine if passed number is even.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>True if passed number is even, else false.</returns>
        [BindableMethod]
        public static bool IsEven(long number) => number % 2 == 0;

        /// <summary>
        ///     Determine if passed number is odd.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>True if passed number is odd, else false.</returns>
        [BindableMethod]
        public static bool IsOdd(long number) => number % 2 != 0;

        /// <summary>
        ///     Gets current time.
        /// </summary>
        /// <returns>Current time.</returns>
        [BindableMethod]
        public static DateTimeOffset Now() => DateTimeOffset.Now;

        /// <summary>
        ///     Gets current time in UTC.
        /// </summary>
        /// <returns>Current time (UTC).</returns>
        [BindableMethod]
        public static DateTimeOffset UtcNow() => DateTimeOffset.UtcNow;

        /// <summary>
        ///     Gets the day of month.
        /// </summary>
        /// <param name="datetime">The datetime</param>
        /// <returns>Day of month.</returns>
        [BindableMethod]
        public static int GetDay([InjectReferenceTime] DateTimeOffset datetime) => datetime.Day;

        /// <summary>
        ///     Gets month of year.
        /// </summary>
        /// <param name="datetime">The datetime</param>
        /// <returns>Month of year.</returns>
        [BindableMethod]
        public static int GetMonth([InjectReferenceTime] DateTimeOffset datetime) => datetime.Month;

        /// <summary>
        ///     Gets year.
        /// </summary>
        /// <param name="datetime">The datetime</param>
        /// <returns>Year</returns>
        [BindableMethod]
        public static int GetYear([InjectReferenceTime] DateTimeOffset datetime) => datetime.Year;

        /// <summary>
        ///     Gets second.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns>Second</returns>
        [BindableMethod]
        public static int GetSecond([InjectReferenceTime] DateTimeOffset datetime) => datetime.Second;

        /// <summary>
        ///     Gets minute.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        [BindableMethod]
        public static int GetMinute([InjectReferenceTime] DateTimeOffset datetime) => datetime.Minute;

        /// <summary>
        ///     Gets hour.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns>Hour.</returns>
        [BindableMethod]
        public static int GetHour([InjectReferenceTime] DateTimeOffset datetime) => datetime.Hour;

        /// <summary>
        ///     Gets the time
        /// </summary>
        /// <param name="datetime">The reference time.</param>
        /// <returns></returns>
        [BindableMethod]
        public static long GetTime([InjectReferenceTime] DateTimeOffset datetime) => datetime.TimeOfDay.Ticks;

        /// <summary>
        ///     Creates the time.
        /// </summary>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <returns>Time based on passed arguments.</returns>
        [BindableMethod]
        public static long Time(int hour, int minute, int second) => new TimeSpan(hour, minute, second).Ticks;

        /// <summary>
        ///     Gets week of month.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <param name="type">Type of calcuation to get value.</param>
        /// <returns>Week of month.</returns>
        [BindableMethod]
        public static int GetWeekOfMonth([InjectReferenceTime] DateTimeOffset datetime, string type = "")
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
        ///     Gets day of year.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns>Day of year.</returns>
        [BindableMethod]
        public static int GetDayOfYear([InjectReferenceTime] DateTimeOffset datetime) => datetime.DayOfYear;

        /// <summary>
        ///     Gets day of week.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns>Day of week.</returns>
        [BindableMethod]
        public static int GetDayOfWeek([InjectReferenceTime] DateTimeOffset datetime) => (int) datetime.DayOfWeek;

        /// <summary>
        ///     Determine if the date is working day.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns>True if it's working day, else false.</returns>
        [BindableMethod]
        public static bool IsWorkingDay([InjectReferenceTime] DateTimeOffset datetime)
            => WorkingDays.Contains(datetime.DayOfWeek);

        /// <summary>
        ///     Gets the random number.
        /// </summary>
        /// <returns>Random number.</returns>
        [BindableMethod]
        [DoNotCache]
        public long GetRandomValue() => _random.Next();

        /// <summary>
        ///     Gets the random number.
        /// </summary>
        /// <param name="min">Min value of number can be outputed.</param>
        /// <param name="max">Max value of number can be outputed.</param>
        /// <returns>Random number.</returns>
        [BindableMethod]
        [DoNotCache]
        public int GetRandomValue(int min, int max) => _random.Next(min, max);

        /// <summary>
        /// Gets the random number that can be other for each instance.
        /// </summary>
        /// <returns>Random number.</returns>
        [BindableMethod]
        public int GetInstanceRandomValue() => _instanceRandomNumber;

        /// <summary>
        /// Gets the random number that can be other for each instance.
        /// </summary>
        /// <param name="min">Min value of number can be outputed.</param>
        /// <param name="max">Max value of number can be outputed.</param>
        /// <returns>Random number.</returns>
        [BindableMethod]
        public long GetInstanceRandomValue(int min, int max) => min + (_instanceRandomNumber % (max - min));

        /// <summary>
        /// Draws occurence for specific PartOfDate (ie. N x times in a day when part of date had been chosen to hours)
        /// </summary>
        /// <param name="referenceTime">The datetime.</param>
        /// <param name="type">The type.</param>
        /// <param name="index">In scope order.</param>
        /// <param name="count">Amount of occurences of this method in scope.</param>
        /// <returns>Drawed value.</returns>
        [BindableMethod]
        [DoNotCache]
        public int NRandomTime([InjectReferenceTime] DateTimeOffset referenceTime, [InjectPartOfDateType] PartOfDate type, [InjectOccurenceOrder] int index, [InjectOccurencesAmount] int count)
        {
            int minValue;
            var divisor = 0;

            switch (type)
            {
                case PartOfDate.Hours:
                    if (_lastPartOfDayValue != -1 && _lastPartOfDayValue == referenceTime.Day && _randomizedValues.Count == count)
                        return _randomizedValues[index];
                    else if(_lastPartOfDayValue != -1 && _lastPartOfDayValue != referenceTime.Day)
                        _randomizedValues.Clear();

                    _lastPartOfDayValue = referenceTime.Day;
                    minValue = 0;
                    divisor = 24;
                    break;
                case PartOfDate.Minutes:
                    if (_lastPartOfDayValue != -1 && _lastPartOfDayValue == referenceTime.Hour && _randomizedValues.Count == count)
                        return _randomizedValues[index];
                    else if (_lastPartOfDayValue != -1 && _lastPartOfDayValue != referenceTime.Hour)
                        _randomizedValues.Clear();

                    _lastPartOfDayValue = referenceTime.Hour;
                    minValue = 0;
                    divisor = 60;
                    break;
                case PartOfDate.Seconds:
                    if (_lastPartOfDayValue != -1 && _lastPartOfDayValue == referenceTime.Minute && _randomizedValues.Count == count)
                        return _randomizedValues[index];
                    else if (_lastPartOfDayValue != -1 && _lastPartOfDayValue != referenceTime.Minute)
                        _randomizedValues.Clear();

                    _lastPartOfDayValue = referenceTime.Minute;
                    minValue = 0;
                    divisor = 60;
                    break;
                case PartOfDate.Years:
                    throw new NotSupportedException("years");
                case PartOfDate.Months:
                    if (_lastPartOfDayValue != -1 && _lastPartOfDayValue == referenceTime.Year && _randomizedValues.Count == count)
                        return _randomizedValues[index];
                    else if (_lastPartOfDayValue != -1 && _lastPartOfDayValue != referenceTime.Year)
                        _randomizedValues.Clear();

                    _lastPartOfDayValue = referenceTime.Year;
                    minValue = 1;
                    divisor = 12;
                    break;
                case PartOfDate.DaysOfMonth:
                    if (_lastPartOfDayValue != -1 && _lastPartOfDayValue == referenceTime.Month && _randomizedValues.Count == count)
                        return _randomizedValues[index];
                    else if (_lastPartOfDayValue != -1 && _lastPartOfDayValue != referenceTime.Month)
                        _randomizedValues.Clear();

                    _lastPartOfDayValue = referenceTime.Month;
                    minValue = 1;
                    divisor = DateTime.DaysInMonth(referenceTime.Year, referenceTime.Month);
                    break;
                default:
                    throw new NotSupportedException(nameof(type));
            }

            var value = GetRandomValue(minValue, divisor);
            if (_randomizedValues.Count > index)
            {
                while (value == _randomizedValues[index])
                {
                    value = GetRandomValue(minValue, divisor);
                }
                _randomizedValues[index] = value;
            }
            else
            {
                if (index > 0)
                {
                    while (_randomizedValues.Contains(value))
                    {
                        value = GetRandomValue(minValue, divisor);
                    }
                }
                _randomizedValues.Add(value);
            }

            return value;
        }
    }
}