﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TQL.RDL.Evaluator.Attributes;
using TQL.RDL.Evaluator.ErrorHandling;

namespace TQL.RDL.Converter.Tests
{
    [BindableClass]
    [TestClass]
    public class EvaluatorTests
    {
        private static bool _staticMethod1Called;
        private static bool _staticMethod2Called;
        
        [TestMethod]
        public void CodeGenerationVisitor_WithAlwaysFalseNode_ShouldReturnNull()
        {
            var evaluator = TestHelper.ToEvaluator("repeat every days where GetDay() in (21,22,23,24) and 3 = 4 and GetYear() < 2100 start at '01.01.2000' stop at '05.05.2100'");
            
            DateTimeOffset? refTime;
            var count = 0;
            do
            {
                refTime = evaluator.NextFire();
                count += 1;
            }
            while (refTime != null);

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void CodeGenerationVisitor_ComposeFunctionCall_ShouldPass()
        {
            var response =
                TestHelper.Convert<EvaluatorTests>(
                    "repeat every hours where TestMethodWithDateTimeOffset(GetDate(), GetYear()) and TestMethodWithDateTimeOffset(GetDate()) start at '07.06.2016 22:00:00'");

            var machine = response.Output;

            machine.NextFire();

            Assert.IsTrue(_staticMethod1Called);
            Assert.IsTrue(_staticMethod2Called);
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluateSimpleStartAtStopAt_ShouldPass()
        {
            var evaluator = TestHelper.ToEvaluator("repeat every hours start at '21.05.2012 05:00:00' stop at '21.05.2012 12:00:00'");

            var refTime = DateTimeOffset.Parse("21.05.2012 05:00:00");
            var datetime = default(DateTimeOffset?);
            while((datetime = evaluator.NextFire()).HasValue)
            {
                Assert.AreEqual(refTime, datetime);
                refTime = refTime.AddHours(1);
            }

            Assert.AreEqual(null, datetime);
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluateSimpleWithModifiedRepetiotion_ShouldPass()
        {
            var evaluator = TestHelper
                .ToEvaluator("repeat every 2 hours start at '21.05.2012 05:00:00' stop at '21.05.2012 12:00:00'");

            var refTime = DateTimeOffset.Parse("21.05.2012 05:00:00");
            var datetime = default(DateTimeOffset?);

            while ((datetime = evaluator.NextFire()).HasValue)
            {
                Assert.AreEqual(refTime, datetime);
                refTime = refTime.AddHours(2);
            }

            Assert.AreEqual(null, datetime);
        }

        [TestMethod]
        public void CodeGenerationVisitor_RunsAfterEveryTwoWeeks()
        {
            EvaluateQuery("repeat every days where (GetDay() >= 1 and GetDay() <= 7) or (GetDay() > 14 and GetDay() <= 21) start at '11.12.2016 20:17:57'", string.Empty, string.Empty,
                x => x == DateTimeOffset.Parse("15.12.2016 20:17:57"));
            
            EvaluateQuery("repeat every days where GetWeekOfMonth() in (1,3) start at '11.12.2016 20:17:57'", string.Empty, string.Empty,
                x => x == DateTimeOffset.Parse("15.12.2016 20:17:57")); //GetWeekOfMonth() in (1,3)
        }

        [TestMethod]
        public void CodeGenerationVisitor_CaseWhen_ShouldPass()
        {
            EvaluateQuery(@"repeat every days where 1 = (
                case 
                    when (GetDay() in (1,5)) then (1) 
                    when (GetDay() in (10,15)) then (1) 
                    else (0) esac) start at {0} stop at {1}", 
                "'01.01.2012 00:00:00'", 
                "'01.02.2012 00:00:00'",
                x => x == DateTimeOffset.Parse("01.01.2012 00:00:00"),
                x => x == DateTimeOffset.Parse("05.01.2012 00:00:00"),
                x => x == DateTimeOffset.Parse("10.01.2012 00:00:00"),
                x => x == DateTimeOffset.Parse("15.01.2012 00:00:00"));

            //always false query
            EvaluateQuery("repeat every 1 days where 1 = (case when (3 = 4) then (1) when (3 = 4) then (1) else (0) esac) start at {0} stop at {1}", "'01.01.2012 00:00:00'", "'01.02.2012 00:00:00'");
            EvaluateQuery("repeat every 1 days where 1 = (case when (GetDay() > 2 and GetDay() < 5) then (1) else (0) esac) start at {0} stop at {1}", "'01.01.2012 00:00:00'", "'01.02.2012 00:00:00'",
                x => x == DateTimeOffset.Parse("03.01.2012 00:00:00"),
                x => x == DateTimeOffset.Parse("04.01.2012 00:00:00"));
        }

        [TestMethod]
        public void CodeGenerationVisitor_CaseWhen_EvaluationDependsOnWeekOfMonth_ShouldPass()
        {
            EvaluateQuery(@"repeat every days where 1 = 
                (case
                    when GetWeekOfMonth() = 1 and GetDayOfWeek() in (2,3) then 1 
                    when GetWeekOfMonth() = 3 and GetDayOfWeek() in (4,5) then 1 
                    else 0 esac) start at {0}",
                "'01.12.2016 00:00:00'",
                string.Empty,
                x => x == DateTimeOffset.Parse("06.12.2016 00:00:00"),
                x => x == DateTimeOffset.Parse("07.12.2016 00:00:00"),
                x => x == DateTimeOffset.Parse("15.12.2016 00:00:00"),
                x => x == DateTimeOffset.Parse("16.12.2016 00:00:00"));
        }

        [TestMethod]
        public void CodeGenerationVisitor_CaseWhen_Simple_ShouldPass()
        {
            EvaluateQuery("repeat every days where 1 = (case when 2 = 3 then 1 when 4 = 5 then 1 else 1 esac) start at {0} stop at {0}",
                "'01.12.2016 00:00:00'",
                string.Empty,
                x => x == DateTimeOffset.Parse("01.12.2016 00:00:00"));
        }

        [TestMethod]
        public void CodeGenerationVisitor_ModuloOp_ShouldPass()
        {
            EvaluateQuery("repeat every days where GetDay() % 3 = 0 and GetDayOfYear() % 3 = 0 start at '23.12.2016 00:00:00'", string.Empty, string.Empty);
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluateNullWhenStopAtReached_ShouldReturnNull()
        {
            EvaluateQuery("repeat every 2 hours start at '21.05.2012 13:00:00' stop at '21.05.2012 12:00:00'", string.Empty, string.Empty,
                (x) => x == null,
                (x) => x == null);
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluateBasicOperators_ShouldPass()
        {
            EvaluateQuery("repeat every hours where 1 + 2 = 3 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.05.2012 15:00:00'", 
                x => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every hours where 5 - 4 = 1 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.05.2012 15:00:00'",
                x => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every hours where 5 * 4 = 19 + 1 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.05.2012 15:00:00'",
                x => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every hours where 20 / 2 = 50 / 5 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.05.2012 15:00:00'",
                x => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every hours where 20 + (2 * 5) - 4 + 1 = 20 + 6 + 1 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.05.2012 15:00:00'",
                x => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every days where GetDay() * GetYear() = 29 * 2012 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'31.05.2012 15:00:00'",
                x => x == DateTimeOffset.Parse("29.05.2012 13:00:00"));

            EvaluateQuery("repeat every 2 days start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.06.2012 23:00:00'",
                x => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("23.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("25.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("27.05.2012 13:00:00"));
        }

        [TestMethod]
        public void CodeGenerationVisitor_InOperator_ShouldPass()
        {
            EvaluateQuery("repeat every days where GetDay() in (22,25,27) start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'31.05.2012 15:00:00'",
                x => x == DateTimeOffset.Parse("22.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("25.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("27.05.2012 13:00:00"));

            EvaluateQuery("repeat every days where GetDay() not in (22,25,27) start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'31.05.2012 15:00:00'",
                x => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("23.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("24.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("26.05.2012 13:00:00"));

            EvaluateQuery("repeat every days where GetDay() not in (22,25,27) start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'31.05.2012 15:00:00'",
                x => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("23.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("24.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("26.05.2012 13:00:00"));
        }

        [TestMethod]
        public void CodeGenerationVisitor_ComplexConditions_ShouldPass()
        {
            EvaluateQuery(@"
                repeat every minutes where 
                    (GetHour() = 7 and GetMinute() = 0 and GetSecond() = 0) or 
                    (GetHour() = 8 and GetMinute() = 30 and GetSecond() = 0) 
                start at '{0}'", "04.01.2017", string.Empty,
                x => x == DateTimeOffset.Parse("04.01.2017 07:00:00"),
                x => x == DateTimeOffset.Parse("04.01.2017 08:30:00"),
                x => x == DateTimeOffset.Parse("05.01.2017 07:00:00"));
        }

        [TestMethod]
        public void CodeGenerationVisitor_CaseWhenConditions_ShouldPass()
        {
            EvaluateQuery(@"
                repeat every minutes where 1 = 
                    (case 
                        when GetHour() = 7 and GetMinute() = 0 and GetSecond() = 0
                        then 1
                        when GetHour() = 8 and GetMinute() = 30 and GetSecond() = 0
                        then 1
                        else 0
                    esac) start at '04.01.2017' stop at '05.01.2017 08:00:00'
            ",
            string.Empty,
            string.Empty,
            x => x == DateTimeOffset.Parse("04.01.2017 07:00:00"),
            x => x == DateTimeOffset.Parse("04.01.2017 08:30:00"),
            x => x == DateTimeOffset.Parse("05.01.2017 07:00:00"));
        }

        [TestMethod]
        public void CodeGenerationVisitor_WithDayOfWeeksInsteadOfNumbers_ShouldPass()
        {
            EvaluateQuery(@"repeat every days where GetDayOfWeek() = monday or GetDayOfWeek() = frIday start at '12.01.2017 00:00:00'",
                string.Empty,
                string.Empty,
                x => x == DateTimeOffset.Parse("13.01.2017 00:00:00"),
                x => x == DateTimeOffset.Parse("16.01.2017 00:00:00"));

            EvaluateQuery(@"repeat every days where GetDayOfWeek() in (monday, frIday) start at '12.01.2017 00:00:00'",
                string.Empty,
                string.Empty,
                x => x == DateTimeOffset.Parse("13.01.2017 00:00:00"),
                x => x == DateTimeOffset.Parse("16.01.2017 00:00:00"));
        }

        [TestMethod]
        public void CodeGenerationVisitor_WithDay()
        {
            EvaluateQuery(@"
                repeat every minutes where 1 = 
                    (case 
                        when GetDay() = 1 and GetHour() between 9 and 11
                        then GetMinute() % 5 = 0
                        when IsLastDayOfMonth()
                        then GetHour() = 5 and GetMinute() = 0 and GetSecond() = 0 
                        else 0
                    esac)
                start at '01.01.2017' stop at '01.02.2017'
            ",
            string.Empty, 
            string.Empty,
            x => x == DateTimeOffset.Parse("01.01.2017 09:00:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 09:05:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 09:10:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 09:15:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 09:20:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 09:25:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 09:30:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 09:35:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 09:40:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 09:45:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 09:50:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 09:55:00"),

            x => x == DateTimeOffset.Parse("01.01.2017 10:00:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 10:05:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 10:10:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 10:15:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 10:20:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 10:25:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 10:30:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 10:35:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 10:40:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 10:45:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 10:50:00"),
            x => x == DateTimeOffset.Parse("01.01.2017 10:55:00"));
        }

        [TestMethod]
        public void CodeGenerationVisitor_GetSpecificDaysOfMonthThatAlsoAreSpecificDaysOfWeek_ShouldPass()
        {
            EvaluateQuery(@"repeat every days where GetDay() in (21,22) and GetDayOfWeek() in (1,2) start at '13.01.2017 00:00:00'", 
                string.Empty, 
                string.Empty,
                x => true,
                x => true,
                x => true);
        }

        [BindableMethod]
        public static bool TestMethodWithDateTimeOffset(DateTimeOffset? date)
        {
            _staticMethod1Called = true;
            return true;
        }

        [BindableMethod]
        public static bool TestMethodWithDateTimeOffset(DateTimeOffset? date, long? year)
        {
            _staticMethod2Called = true;
            return true;
        }

        [BindableMethod]
        public static int GetYear([InjectReferenceTime] DateTimeOffset datetime) => datetime.Year;

        [BindableMethod]
        public static DateTimeOffset GetDate([InjectReferenceTime] DateTimeOffset datetime) => datetime;

        private void EvaluateQuery(string query, string startAt, string stopAt, params Func<DateTimeOffset?, bool>[] funcs)
        {
            var response = TestHelper.Convert(string.Format(query, startAt, stopAt));

            DateTimeOffset? datetime;
            var index = 0;

            Assert.IsFalse(response.Messages.Any(f => f.Level == MessageLevel.Error));

            var evaluator = response.Output;

            while (index < funcs.Length)
            {
                datetime = evaluator.NextFire();
                Assert.IsTrue(funcs[index](datetime));
                index += 1;
            }

            Assert.IsTrue(index == funcs.Length);
        }
    }
}