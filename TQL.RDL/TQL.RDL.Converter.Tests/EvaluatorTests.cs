using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TQL.Common.Timezone;
using TQL.Interfaces;
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
        public void CodeGenerationVisitor_WithCustomDestinationTimezone_ShouldEvaluate()
        {
            var evaluator =
                TestHelper.Convert<DefaultMethodsAggregator>("repeat every days where GetHour() = 10 start at '08/07/2017 10:00:00'",
                    TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time"), TimeZoneInfo.Local).Output;

            Assert.AreEqual(DateTimeOffset.Parse("08/07/2017 20:00:00 +02:00"), evaluator.NextFire());
        }

        [TestMethod]
        public void CodeGenerationVisitor_WithAlwaysFalseNode_ShouldReturnNull()
        {
            var evaluator =
                TestHelper.ToEvaluator(
                    "repeat every days where GetDay() in (21,22,23,24) and 3 = 4 and GetYear() < 2100 start at '01.01.2000' stop at '05.05.2100'");

            DateTimeOffset? refTime;
            var count = 0;
            do
            {
                refTime = evaluator.NextFire();
                count += 1;
            } while (refTime != null);

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void CodeGenerationVisitor_DaylightSavingTime_SpringTime_HoursResolution()
        {
            var response =
                TestHelper.Convert<DefaultMethodsAggregator>(
                    "repeat every 1 hours start at '26.03.2017 00:00:00'", TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

            var machine = response.Output;
            
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 00:00:00 +01:00"), machine.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 01:00:00 +01:00"), machine.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 03:00:00 +02:00"), machine.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 04:00:00 +02:00"), machine.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 05:00:00 +02:00"), machine.NextFire());
        }

        [TestMethod]
        public void CodeGenerationVisitor_DaylightSavingTime_SpringTime_MinutesResolution()
        {
            var response =
                TestHelper.Convert<DefaultMethodsAggregator>(
                    "repeat every 3 minutes start at '26.03.2017 01:58:00'", TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

            var machine = response.Output;

            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 01:58:00 +01:00"), machine.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 03:01:00 +02:00"), machine.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 03:04:00 +02:00"), machine.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 03:07:00 +02:00"), machine.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("26.03.2017 03:10:00 +02:00"), machine.NextFire());
        }

        [TestMethod]
        public void CodeGenerationVisitor_DaylightSavingTime_WinterTime_HoursResolution()
        {
            var response =
                TestHelper.Convert<DefaultMethodsAggregator>(
                    "repeat every 1 hours start at '29.10.2017 00:00:00'", TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

            var machine = response.Output;

            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 00:00:00 +02:00"), machine.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 01:00:00 +02:00"), machine.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 02:00:00 +02:00"), machine.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 02:00:00 +01:00"), machine.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 03:00:00 +01:00"), machine.NextFire());
        }

        [TestMethod]
        public void CodeGenerationVisitor_DaylightSavingTime_WinterTime_MinutesResolution()
        {
            var response =
                TestHelper.Convert<DefaultMethodsAggregator>(
                    "repeat every 65 minutes start at '29.10.2017 01:55:00'", TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

            var machine = response.Output;

            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 01:55:00 +02:00"), machine.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 02:00:00 +01:00"), machine.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 03:05:00 +01:00"), machine.NextFire());

            response =
                TestHelper.Convert<DefaultMethodsAggregator>(
                    "repeat every 65 minutes start at '29.10.2017 01:35:00'", TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

            machine = response.Output;

            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 01:35:00 +02:00"), machine.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 02:40:00 +02:00"), machine.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 02:45:00 +01:00"), machine.NextFire());
        }

        [TestMethod]
        public void CodeGenerationVisitor_DaylightSavingTime_WinterTime_DaysResolution()
        {
            var response =
                TestHelper.Convert<DefaultMethodsAggregator>(
                    "repeat every days start at '29.10.2017 00:00:00'", TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));

            var machine = response.Output;

            Assert.AreEqual(DateTimeOffset.Parse("29.10.2017 00:00:00 +02:00"), machine.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("30.10.2017 00:00:00 +01:00"), machine.NextFire());
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
            var evaluator =
                TestHelper.ToEvaluator("repeat every hours start at '21.05.2012 05:00:00' stop at '21.05.2012 12:00:00'");

            var refTime = DateTimeOffset.Parse("21.05.2012 05:00:00");
            var datetime = default(DateTimeOffset?);
            while ((datetime = evaluator.NextFire()).HasValue)
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
            EvaluateQuery(
                "repeat every days where (GetDay() >= 1 and GetDay() <= 7) or (GetDay() > 14 and GetDay() <= 21) start at '11.12.2016 20:17:57'",
                string.Empty, string.Empty,
                x => x == DateTimeOffset.Parse("15.12.2016 20:17:57"));

            EvaluateQuery("repeat every days where GetWeekOfMonth() in (1,3) start at '11.12.2016 20:17:57'",
                string.Empty, string.Empty,
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
            EvaluateQuery(
                "repeat every 1 days where 1 = (case when (3 = 4) then (1) when (3 = 4) then (1) else (0) esac) start at {0} stop at {1}",
                "'01.01.2012 00:00:00'", "'01.02.2012 00:00:00'");
            EvaluateQuery(
                "repeat every 1 days where 1 = (case when (GetDay() > 2 and GetDay() < 5) then (1) else (0) esac) start at {0} stop at {1}",
                "'01.01.2012 00:00:00'", "'01.02.2012 00:00:00'",
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
            EvaluateQuery(
                "repeat every days where 1 = (case when 2 = 3 then 1 when 4 = 5 then 1 else 1 esac) start at {0} stop at {0}",
                "'01.12.2016 00:00:00'",
                string.Empty,
                x => x == DateTimeOffset.Parse("01.12.2016 00:00:00"));
        }

        [TestMethod]
        public void CodeGenerationVisitor_ModuloOp_ShouldPass()
        {
            EvaluateQuery(
                "repeat every days where GetDay() % 3 = 0 and GetDayOfYear() % 3 = 0 start at '23.12.2016 00:00:00'",
                string.Empty, string.Empty);
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluateNullWhenStopAtReached_ShouldReturnNull()
        {
            EvaluateQuery("repeat every 2 hours start at '21.05.2012 13:00:00' stop at '21.05.2012 12:00:00'",
                string.Empty, string.Empty,
                x => x == null,
                x => x == null);
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluateBasicOperators_ShouldPass()
        {
            EvaluateQuery("repeat every hours where 1 + 2 = 3 start at {0} stop at {1}", "'21.05.2012 13:00:00'",
                "'21.05.2012 15:00:00'",
                x => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every hours where 5 - 4 = 1 start at {0} stop at {1}", "'21.05.2012 13:00:00'",
                "'21.05.2012 15:00:00'",
                x => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every hours where 5 * 4 = 19 + 1 start at {0} stop at {1}", "'21.05.2012 13:00:00'",
                "'21.05.2012 15:00:00'",
                x => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every hours where 20 / 2 = 50 / 5 start at {0} stop at {1}", "'21.05.2012 13:00:00'",
                "'21.05.2012 15:00:00'",
                x => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every hours where 20 + (2 * 5) - 4 + 1 = 20 + 6 + 1 start at {0} stop at {1}",
                "'21.05.2012 13:00:00'", "'21.05.2012 15:00:00'",
                x => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                x => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every days where GetDay() * GetYear() = 29 * 2012 start at {0} stop at {1}",
                "'21.05.2012 13:00:00'", "'31.05.2012 15:00:00'",
                x => x == DateTimeOffset.Parse("29.05.2012 13:00:00"));

            EvaluateQuery("repeat every 2 days start at {0} stop at {1}", "'21.05.2012 13:00:00'",
                "'21.06.2012 23:00:00'",
                x => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("23.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("25.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("27.05.2012 13:00:00"));
        }

        [TestMethod]
        public void CodeGenerationVisitor_InOperator_ShouldPass()
        {
            EvaluateQuery("repeat every days where GetDay() in (22,25,27) start at {0} stop at {1}",
                "'21.05.2012 13:00:00'", "'31.05.2012 15:00:00'",
                x => x == DateTimeOffset.Parse("22.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("25.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("27.05.2012 13:00:00"));

            EvaluateQuery("repeat every days where GetDay() not in (22,25,27) start at {0} stop at {1}",
                "'21.05.2012 13:00:00'", "'31.05.2012 15:00:00'",
                x => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("23.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("24.05.2012 13:00:00"),
                x => x == DateTimeOffset.Parse("26.05.2012 13:00:00"));

            EvaluateQuery("repeat every days where GetDay() not in (22,25,27) start at {0} stop at {1}",
                "'21.05.2012 13:00:00'", "'31.05.2012 15:00:00'",
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
            EvaluateQuery(
                @"repeat every days where GetDayOfWeek() = monday or GetDayOfWeek() = frIday start at '12.01.2017 00:00:00'",
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
            EvaluateQuery(
                @"repeat every days where GetDay() in (21,22) and GetDayOfWeek() in (1,2) start at '13.01.2017 00:00:00'",
                string.Empty,
                string.Empty,
                x => true,
                x => true,
                x => true);
        }

        [TestMethod]
        public void CodeGenerationVisitor_IsSatisfiedBy_ShouldPass()
        {
            var evaluator =
                EvaluateQuery(
                    "repeat every days where GetDay() between 9 and 12 or GetDay() between 15 and 16 start at '01.01.2017'",
                    string.Empty,
                    string.Empty);

            Assert.IsFalse(evaluator.IsSatisfiedBy(DateTimeOffset.Parse("14.01.2017")));
            Assert.IsTrue(evaluator.IsSatisfiedBy(DateTimeOffset.Parse("15.01.2017")));
            Assert.IsFalse(evaluator.IsSatisfiedBy(DateTimeOffset.Parse("16.01.2017")));

            Assert.IsFalse(evaluator.IsSatisfiedBy(DateTimeOffset.Parse("7.01.2017")));
            Assert.IsTrue(evaluator.IsSatisfiedBy(DateTimeOffset.Parse("10.01.2017")));
            Assert.IsFalse(evaluator.IsSatisfiedBy(DateTimeOffset.Parse("13.01.2017")));
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluateGetTimeBetween_ShouldPass()
        {
            var evaluator =
                EvaluateQuery(
                    @"repeat every minutes where GetTime() between Time(8, 30, 0) and Time(11, 30, 0) start at '16.04.2017' stop at '17.04.2017'",
                    string.Empty, string.Empty);

            DateTimeOffset lastlyCalculated = DateTimeOffset.MinValue;
            DateTimeOffset? latest = null;

            Assert.AreEqual(DateTimeOffset.Parse("16.04.2017 08:30:00"), evaluator.NextFire());
            while ((latest = evaluator.NextFire()) != null)
            {
                lastlyCalculated = latest.Value;
            }
            Assert.AreEqual(DateTimeOffset.Parse("16.04.2017 11:29:00"), lastlyCalculated);
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluateWithComplexScope_ShouldPass()
        {
            var evaluator = EvaluateQuery(@"
                repeat every hours where (
                    GetHour() in (10, 11)) or 1 = 
                    (case
                        when GetHour() between 14 and 16
                        then 1
                        else 0
                    esac) start at '01.04.2017' stop at '02.04.2017'
            ", string.Empty, string.Empty);

            Assert.AreEqual(DateTimeOffset.Parse("01.04.2017 10:00:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("01.04.2017 11:00:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("01.04.2017 14:00:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("01.04.2017 15:00:00"), evaluator.NextFire());
            Assert.AreEqual(null, evaluator.NextFire());
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluateComplexExpression_ShouldPass()
        {
            var evaluator = EvaluateQuery(@"
                repeat every minutes where 1 = 
                    (case 
                        when GetWeekOfMonth() in (1,3) and GetDayOfWeek() = monday
                        then GetTime() between Time(8, 30, 0) and Time(11, 30, 0)
                        when GetWeekOfMonth() in (2,4) and GetDayOfWeek() in (tuesday, sunday)
                        then GetTime() = Time(12, 0, 0)
                        else 0
                    esac) start at '01.04.2017' stop at '30.04.2017'
            ", string.Empty, string.Empty);

            DateTimeOffset previous = DateTimeOffset.MinValue;
            DateTimeOffset? latest = null;

            previous = evaluator.NextFire().Value;
            Assert.AreEqual(DateTimeOffset.Parse("03.04.2017 08:30:00"), previous);
            while ((latest = evaluator.NextFire()).Value.Day == previous.Day)
            {
                previous = latest.Value;
            }

            Assert.AreEqual(DateTimeOffset.Parse("09.04.2017 12:00:00"), latest);
            Assert.AreEqual(DateTimeOffset.Parse("11.04.2017 12:00:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("17.04.2017 08:30:00"), evaluator.NextFire());

            previous = evaluator.NextFire().Value;
            Assert.AreEqual(DateTimeOffset.Parse("17.04.2017 08:31:00"), previous);
            while ((latest = evaluator.NextFire()).Value.Day == previous.Day)
            {
                previous = latest.Value;
            }

            Assert.AreEqual(DateTimeOffset.Parse("17.04.2017 11:29:00"), previous);

            Assert.AreEqual(DateTimeOffset.Parse("23.04.2017 12:00:00"), latest);
            Assert.AreEqual(DateTimeOffset.Parse("25.04.2017 12:00:00"), evaluator.NextFire());
            Assert.IsNull(evaluator.NextFire());
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluateEveryNthDay_ShouldPass()
        {
            var evaluator = EvaluateQuery(
                @"repeat every days where 1 = EveryNth(21, 'day') start at '01.04.2017' stop at '01.06.2017'", string.Empty, string.Empty);

            Assert.AreEqual(DateTimeOffset.Parse("01.04.2017 00:00:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("22.04.2017 00:00:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("13.05.2017 00:00:00"), evaluator.NextFire());
            Assert.IsNull(evaluator.NextFire());
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluteTwiceADayOnDifferentTime()
        {
            var evaluator = EvaluateQuery(
                @"repeat every hours where GetHour() in (NRandomTime(), NRandomTime()) start at '13.08.2017'", string.Empty, string.Empty);

            Assert.AreEqual(13, evaluator.NextFire().Value.Day);
            Assert.AreEqual(13, evaluator.NextFire().Value.Day);
            Assert.AreEqual(14, evaluator.NextFire().Value.Day);
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluteTwiceAMinuteOnDifferentTime()
        {
            var evaluator = EvaluateQuery(
                @"repeat every seconds where GetSecond() in (NRandomTime(), NRandomTime()) start at '13.08.2017'", string.Empty, string.Empty);

            Assert.AreEqual(0, evaluator.NextFire().Value.Minute);
            Assert.AreEqual(0, evaluator.NextFire().Value.Minute);
            Assert.AreEqual(1, evaluator.NextFire().Value.Minute);
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluteTwiceAnHourOnDifferentTime()
        {
            var evaluator = EvaluateQuery(
                @"repeat every minutes where GetMinute() in (NRandomTime(), NRandomTime()) start at '13.08.2017'", string.Empty, string.Empty);

            Assert.AreEqual(0, evaluator.NextFire().Value.Hour);
            Assert.AreEqual(0, evaluator.NextFire().Value.Hour);
            Assert.AreEqual(1, evaluator.NextFire().Value.Hour);
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluteTwiceAMonthOnDifferentTime()
        {
            var evaluator = EvaluateQuery(
                @"repeat every days where GetDay() in (NRandomTime(), NRandomTime()) start at '01.08.2017'", string.Empty, string.Empty);

            Assert.AreEqual(8, evaluator.NextFire().Value.Month);
            Assert.AreEqual(8, evaluator.NextFire().Value.Month);
            Assert.AreEqual(9, evaluator.NextFire().Value.Month);
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluteTwiceAYearOnDifferentTime()
        {
            var evaluator = EvaluateQuery(
                @"repeat every months where GetMonth() in (NRandomTime(), NRandomTime()) start at '01.01.2017'", string.Empty, string.Empty);

            Assert.AreEqual(2017, evaluator.NextFire().Value.Year);
            Assert.AreEqual(2017, evaluator.NextFire().Value.Year);
            Assert.AreEqual(2018, evaluator.NextFire().Value.Year);
        }

        [TestMethod]
        public void CodeGenerationVisitor_Evaluate17thNotWhenIsWeekend()
        {
            var evaluator = EvaluateQuery(
                @"repeat every days where 1 = (
                    case 
                        when IsWorkingDay() and GetDay() = 17
                        then 1
                        when GetDayOfWeek(17) = saturday
                        then GetDay() = 16
                        when GetDayOfWeek(17) = sunday
                        then GetDay() = 15
                        else 0
                    esac) start at '01.01.2017'", string.Empty, string.Empty);

            Assert.AreEqual(DateTimeOffset.Parse("17.01.2017"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("17.02.2017"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("17.03.2017"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("17.04.2017"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("17.05.2017"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("16.06.2017"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("17.07.2017"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("17.08.2017"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("15.09.2017"), evaluator.NextFire());
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluateEachWeekdayDifferentSchedule_ShouldPass()
        {
            var evaluator = EvaluateQuery(
                @"repeat every hours where 1 = (
                    case 
                        when GetDayOfWeek() in (monday, tuesday)
                        then GetHour() = 10
                        when GetDayOfWeek() = wednesday
                        then GetHour() = 0
                        when GetDayOfWeek() = thursday
                        then GetHour() = 4
                        when GetDayOfWeek() = friday
                        then GetHour() = 6
                        else 0
                    esac) start at '01.08.2017'", string.Empty, string.Empty);

            Assert.AreEqual(DateTimeOffset.Parse("01.08.2017 10:00:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("02.08.2017 00:00:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("03.08.2017 04:00:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("04.08.2017 06:00:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("07.08.2017 10:00:00"), evaluator.NextFire());
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

        private IFireTimeEvaluator EvaluateQuery(string query, string startAt, string stopAt,
            params Func<DateTimeOffset?, bool>[] funcs)
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
            return evaluator;
        }
    }
}