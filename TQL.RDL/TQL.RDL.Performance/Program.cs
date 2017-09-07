using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TQL.RDL.Performance
{
    static class Program
    {
        static void Main(string[] args)
        {
            args = new[] {@"repeat every hours where 1 = 
                    (case 
                when GetDayOfWeek() in (monday, tuesday)
                then GetHour() = 10
                when GetDayOfWeek() = wednesday
                then GetHour() = 0
                when GetDayOfWeek() = thursday
                then GetHour() = 4
                when GetDayOfWeek() = friday
                then GetHour() = 6
                else 0
                esac) start at '01.08.2017' stop at '01.08.2040'"};

            Console.WriteLine($@"Timer started at: {DateTimeOffset.UtcNow}");

            Stopwatch queryTimer = new Stopwatch();

            queryTimer.Start();
            var converter = new RdlTimeline<DefaultMethodsAggregator>();
            var response = converter.Convert(new ConvertionRequest<DefaultMethodsAggregator>(args[0], TimeZoneInfo.Local, TimeZoneInfo.Local));
            var evaluator = response.Output;

            var lastElapsed = queryTimer.ElapsedMilliseconds;
            while (evaluator.NextFire() != null)
            {
                if (queryTimer.ElapsedMilliseconds - lastElapsed > 1000)
                {
                    Console.WriteLine($@"Timer started at: {DateTimeOffset.UtcNow}");
                    lastElapsed = queryTimer.ElapsedMilliseconds;
                }
            }

            queryTimer.Stop();

            Console.WriteLine($@"Timer stopped: {queryTimer.Elapsed}");
        }
    }
}
