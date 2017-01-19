﻿using System;
using System.Linq;
using TQL.RDL.Evaluator.Helpers;

namespace TQL.RDL.Evaluator
{
    public sealed class DefaultMethods
    {
        private static readonly DayOfWeek[] WorkingDays = {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday
        };

        private RdlVirtualMachine _machine;

        public DefaultMethods()
        {
            _machine = null;
        }

        public void SetMachine(RdlVirtualMachine machine)
        {
            _machine = machine;
        }

        public static bool IsLastDayOfMonth(DateTimeOffset datetime) => DateTime.DaysInMonth(datetime.Year, datetime.Month) == datetime.Day;

        public bool IsLastDayOfMonth()
        {
            var datetime = _machine.Datetimes.Peek();
            return DateTime.DaysInMonth(datetime.Year, datetime.Month) == datetime.Day;
        }

        public static bool IsDayOfWeek(DateTimeOffset datetime, long dayOfWeek) => datetime.DayOfWeek == (DayOfWeek)dayOfWeek;

        public bool IsDayOfWeek(long dayOfWeek)
        {
            var datetime = _machine.Datetimes.Peek();
            return datetime.DayOfWeek == (DayOfWeek)dayOfWeek;
        }

        public bool IsWorkingDay()
        {
            var datetime = _machine.Datetimes.Peek();
            return WorkingDays.Contains(datetime.DayOfWeek);
        }

        public static bool IsEven(long number) => number % 2 == 0;

        public static bool IsOdd(long number) => !IsEven(number);

        public DateTimeOffset GetDate() => _machine.ReferenceTime;
        public static DateTimeOffset Now() => DateTimeOffset.Now;
        public static DateTimeOffset UtcNow() => DateTimeOffset.UtcNow;
        public DateTimeOffset? LastDate() => _machine.LastlyFound;

        public long GetDay() => _machine.Datetimes.Peek().Day;
        public long GetMonth() => _machine.Datetimes.Peek().Month;
        public long GetYear() => _machine.Datetimes.Peek().Year;
        public long GetSecond() => _machine.Datetimes.Peek().Second;
        public long GetMinute() => _machine.Datetimes.Peek().Minute;
        public long GetHour() => _machine.Datetimes.Peek().Hour;

        public long GetWeekOfMonth(string type = "")
        {
            var time = _machine.Datetimes.Peek();

            switch (type)
            {
                case "simple":
                    break;
                case "calculated":
                    return time.WeekOfMonth(DayOfWeek.Sunday);
                case "iso":
                    break;
            }
            return time.WeekOfMonth();
        }

        public long GetDayOfYear() => _machine.Datetimes.Peek().DayOfYear;
        public long GetDayOfWeek() => (long)_machine.Datetimes.Peek().DayOfWeek;
    }
}
