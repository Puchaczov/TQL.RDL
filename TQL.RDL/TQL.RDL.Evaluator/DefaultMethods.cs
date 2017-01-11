using System;
using System.Linq;

namespace TQL.RDL.Evaluator
{
    public sealed class DefaultMethods
    {
        private RDLVirtualMachine machine;

        public DefaultMethods()
        {
            machine = null;
        }

        private static DayOfWeek[] workingDays = new DayOfWeek[]
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday
        };

        public void SetMachine(RDLVirtualMachine machine)
        {
            this.machine = machine;
        }

        public static bool IsLastDayOfMonth(DateTimeOffset datetime) => DateTime.DaysInMonth(datetime.Year, datetime.Month) == datetime.Day;

        public bool IsLastDayOfMonth()
        {
            var datetime = machine.Datetimes.Peek();
            return DateTime.DaysInMonth(datetime.Year, datetime.Month) == datetime.Day;
        }

        public static bool IsDayOfWeek(DateTimeOffset datetime, long dayOfWeek) => datetime.DayOfWeek == (DayOfWeek)dayOfWeek;

        public bool IsDayOfWeek(long dayOfWeek)
        {
            var datetime = machine.Datetimes.Peek();
            return datetime.DayOfWeek == (DayOfWeek)dayOfWeek;
        }

        public bool IsWorkingDay()
        {
            var datetime = machine.Datetimes.Peek();
            return workingDays.Contains(datetime.DayOfWeek);
        }

        public static bool IsEven(long number) => number % 2 == 0;

        public static bool IsOdd(long number) => !IsEven(number);

        public DateTimeOffset GetDate() => machine.ReferenceTime;
        public static DateTimeOffset Now() => DateTimeOffset.Now;
        public static DateTimeOffset UtcNow() => DateTimeOffset.UtcNow;
        public DateTimeOffset? LastDate() => machine.LastlyFound;

        public long GetDay() => machine.Datetimes.Peek().Day;
        public long GetMonth() => machine.Datetimes.Peek().Month;
        public long GetYear() => machine.Datetimes.Peek().Year;
        public long GetSecond() => machine.Datetimes.Peek().Second;
        public long GetMinute() => machine.Datetimes.Peek().Minute;
        public long GetHour() => machine.Datetimes.Peek().Hour;
        public long GetWeekOfMonth()
        {
            var time = machine.Datetimes.Peek();
            var day = time.Day;

            if (day <= 7)
                return 1;
            else if (day <= 14)
                return 2;
            else if (day <= 21)
                return 3;
            else if (day <= 28)
                return 4;
            else
                return 5;
        }
        public long GetDayOfYear() => machine.Datetimes.Peek().DayOfYear;
        public long GetDayOfWeek() => (long)machine.Datetimes.Peek().DayOfWeek;
    }
}
