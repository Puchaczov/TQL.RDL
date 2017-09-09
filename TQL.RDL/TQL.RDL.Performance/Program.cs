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
            args = new[] {@"
                repeat every minutes where 1 = 
                    (case 
                        when GetWeekOfMonth() in (1,3) and GetDayOfWeek() = monday
                        then GetTime() between Time(8, 30, 0) and Time(11, 30, 0)
                        when GetWeekOfMonth() in (2,4) and GetDayOfWeek() in (tuesday, sunday)
                        then GetTime() = Time(12, 0, 0)
                        else 0
                    esac) start at '01.04.2017' stop at '30.04.2019'"};

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
