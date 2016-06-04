using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TQL.RDL.Evaluator
{
    public class DefaultMethods
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
            DayOfWeek.Friday,
            DayOfWeek.Saturday,
            DayOfWeek.Sunday
        };

        public void SetMachine(RDLVirtualMachine machine)
        {
            this.machine = machine;
        }

        public static bool IsLastDayOfMonth(DateTimeOffset datetime) => DateTime.DaysInMonth(datetime.Year, datetime.Month) == datetime.Day;

        public bool IsLastDayOfMonth()
        {
            var datetime = machine.Datetimes.Peek();
            return datetime.HasValue && DateTime.DaysInMonth(datetime.Value.Year, datetime.Value.Month) == datetime.Value.Day;
        }

        public static bool IsDayOfWeek(DateTimeOffset datetime, int dayOfWeek) => datetime.DayOfWeek == (DayOfWeek)dayOfWeek;

        public bool IsDayOfWeek(int dayOfWeek)
        {
            var datetime = machine.Datetimes.Peek();
            return datetime.HasValue && datetime.Value.DayOfWeek == (DayOfWeek)dayOfWeek;
        }

        public bool IsWorkingDay()
        {
            var datetime = machine.Datetimes.Peek();
            return datetime.HasValue && workingDays.Contains(datetime.Value.DayOfWeek);
        }

        public static bool IsEven(int number) => number % 2 == 0;

        public static bool IsOdd(int number) => !IsEven(number);
    }
}
