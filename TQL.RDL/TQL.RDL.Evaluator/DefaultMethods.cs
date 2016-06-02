using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TQL.RDL.Evaluator
{
    public class DefaultMethods
    {
        public static bool IsLastDayOfMonth(DateTimeOffset datetime) => DateTime.DaysInMonth(datetime.Year, datetime.Month) == datetime.Day;
        public static bool IsDayOfWeek(DateTimeOffset datetime, int dayOfWeek) => datetime.DayOfWeek == (DayOfWeek)dayOfWeek;
        public static bool IsEven(int number) => number % 2 == 0;
        public static bool IsOdd(int number) => !IsEven(number);
    }
}
