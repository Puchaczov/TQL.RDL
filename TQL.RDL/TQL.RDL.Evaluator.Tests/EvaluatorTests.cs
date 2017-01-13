using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TQL.RDL.Evaluator.Enumerators;
using TQL.RDL.Evaluator.Visitors;
using TQL.RDL.Parser;

namespace TQL.RDL.Evaluator.Tests
{
    [TestClass]
    public class EvaluatorTests
    {
        private static bool _staticMethod1Called = false;
        private static bool _staticMethod2Called = false;
        

        [TestMethod]
        public void CodeGenerationVisitor_WithAlwaysFalseNode_ShouldReturnNull()
        {
            var machine = Parse("repeat every days where GetDay() in (21,22,23,24) and 3 = 4 and GetYear() < 2100 start at '01.01.2000' stop at '05.05.2100'");

            machine.ReferenceTime = DateTimeOffset.Parse("01.01.2000");
            var refTime = default(DateTimeOffset?);
            var count = 0;
            do
            {
                refTime = machine.NextFire();
                count += 1;
            }
            while (refTime != null);

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void CodeGenerationVisitor_ComposeFunctionCall_ShouldPass()
        {
            var machine = Parse("repeat every hours where TestMethodWithDateTimeOffset(GetDate(), GetYear()) and TestMethodWithDateTimeOffset(GetDate()) start at '07.06.2016 22:00:00'");
            machine.ReferenceTime = new DateTimeOffset(2016, 6, 7, 22, 0, 0, new TimeSpan());

            machine.NextFire();

            Assert.IsTrue(_staticMethod1Called);
            Assert.IsTrue(_staticMethod2Called);
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluateSimpleStartAtStopAt_ShouldPass()
        {
            var machine = Parse("repeat every hours start at '21.05.2012 05:00:00' stop at '21.05.2012 12:00:00'");

            var refTime = machine.ReferenceTime;
            var datetime = default(DateTimeOffset?);
            while((datetime = machine.NextFire()).HasValue)
            {
                Assert.AreEqual(refTime, datetime);
                refTime = refTime.AddHours(1);
            }

            Assert.AreEqual(null, datetime);
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluateSimpleWithModifiedRepetiotion_ShouldPass()
        {
            var machine = Parse("repeat every 2 hours start at '21.05.2012 05:00:00' stop at '21.05.2012 12:00:00'");

            var refTime = machine.ReferenceTime;
            var datetime = default(DateTimeOffset?);

            while ((datetime = machine.NextFire()).HasValue)
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
                (x) => x == DateTimeOffset.Parse("15.12.2016 20:17:57"));
            
            EvaluateQuery("repeat every days where GetWeekOfMonth() in (1,3) start at '11.12.2016 20:17:57'", string.Empty, string.Empty,
                (x) => x == DateTimeOffset.Parse("15.12.2016 20:17:57")); //GetWeekOfMonth() in (1,3)
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
                (x) => x == DateTimeOffset.Parse("01.01.2012 00:00:00"),
                (x) => x == DateTimeOffset.Parse("05.01.2012 00:00:00"),
                (x) => x == DateTimeOffset.Parse("10.01.2012 00:00:00"),
                (x) => x == DateTimeOffset.Parse("15.01.2012 00:00:00"));

            //always false query
            EvaluateQuery("repeat every 1 days where 1 = (case when (3 = 4) then (1) when (3 = 4) then (1) else (0) esac) start at {0} stop at {1}", "'01.01.2012 00:00:00'", "'01.02.2012 00:00:00'");
            EvaluateQuery("repeat every 1 days where 1 = (case when (GetDay() > 2 and GetDay() < 5) then (1) else (0) esac) start at {0} stop at {1}", "'01.01.2012 00:00:00'", "'01.02.2012 00:00:00'",
                (x) => x == DateTimeOffset.Parse("03.01.2012 00:00:00"),
                (x) => x == DateTimeOffset.Parse("04.01.2012 00:00:00"));
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
                (x) => x == DateTimeOffset.Parse("06.12.2016 00:00:00"),
                (x) => x == DateTimeOffset.Parse("07.12.2016 00:00:00"),
                (x) => x == DateTimeOffset.Parse("15.12.2016 00:00:00"),
                (x) => x == DateTimeOffset.Parse("16.12.2016 00:00:00"));
        }
        
        [TestMethod]
        public void CodeGenerationVisitor_CaseWhen_Simple_ShouldPass()
        {
            EvaluateQuery("repeat every days where 1 = (case when 2 = 3 then 1 when 4 = 5 then 1 else 1 esac) start at {0} stop at {0}",
                "'01.12.2016 00:00:00'",
                string.Empty,
                (x) => x == DateTimeOffset.Parse("01.12.2016 00:00:00"));
        }

        [TestMethod]
        public void CodeGenerationVisitor_ModuloOp_ShouldPass()
        {
            EvaluateQuery("repeat every days where GetDay() % 3 = 0 and GetDayOfYear() % 3 = 0 start at '23.12.2016 00:00:00'", string.Empty, string.Empty);
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluateNullWhenStopAtReached_ShouldReturnNull()
        {
            var machine = Parse("repeat every 2 hours start at '21.05.2012 13:00:00' stop at '21.05.2012 12:00:00'");

            Assert.AreEqual(null, machine.NextFire());
            Assert.AreEqual(null, machine.NextFire());
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluateBasicOperators_ShouldPass()
        {
            EvaluateQuery("repeat every hours where 1 + 2 = 3 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.05.2012 15:00:00'", 
                (x) => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every hours where 5 - 4 = 1 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.05.2012 15:00:00'",
                (x) => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every hours where 5 * 4 = 19 + 1 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.05.2012 15:00:00'",
                (x) => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every hours where 20 / 2 = 50 / 5 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.05.2012 15:00:00'",
                (x) => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every hours where 20 + (2 * 5) - 4 + 1 = 20 + 6 + 1 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.05.2012 15:00:00'",
                (x) => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every days where GetDay() * GetYear() = 29 * 2012 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'31.05.2012 15:00:00'",
                (x) => x == DateTimeOffset.Parse("29.05.2012 13:00:00"));

            EvaluateQuery("repeat every 2 days start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.06.2012 23:00:00'",
                (x) => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("23.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("25.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("27.05.2012 13:00:00"));
        }

        [TestMethod]
        public void CodeGenerationVisitor_InOperator_ShouldPass()
        {
            EvaluateQuery("repeat every days where GetDay() in (22,25,27) start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'31.05.2012 15:00:00'",
                (x) => x == DateTimeOffset.Parse("22.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("25.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("27.05.2012 13:00:00"));

            EvaluateQuery("repeat every days where GetDay() not in (22,25,27) start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'31.05.2012 15:00:00'",
                (x) => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("23.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("24.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("26.05.2012 13:00:00"));

            EvaluateQuery("repeat every days where GetDay() not in (22,25,27) start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'31.05.2012 15:00:00'",
                (x) => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("23.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("24.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("26.05.2012 13:00:00"));
        }

        [TestMethod]
        public void CodeGenerationVisitor_ComplexConditions_ShouldPass()
        {
            EvaluateQuery(@"
                repeat every minutes where 
                    (GetHour() = 7 and GetMinute() = 0 and GetSecond() = 0) or 
                    (GetHour() = 8 and GetMinute() = 30 and GetSecond() = 0) 
                start at '{0}'", "04.01.2017", string.Empty,
                (x) => x == DateTimeOffset.Parse("04.01.2017 07:00:00"),
                (x) => x == DateTimeOffset.Parse("04.01.2017 08:30:00"),
                (x) => x == DateTimeOffset.Parse("05.01.2017 07:00:00"));
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
            (x) => x == DateTimeOffset.Parse("04.01.2017 07:00:00"),
            (x) => x == DateTimeOffset.Parse("04.01.2017 08:30:00"),
            (x) => x == DateTimeOffset.Parse("05.01.2017 07:00:00"));
        }

        [TestMethod]
        public void CodeGenerationVisitor_WithDayOfWeeksInsteadOfNumbers_ShouldPass()
        {
            EvaluateQuery(@"repeat every days where GetDayOfWeek() = monday or GetDayOfWeek() = frIday start at '12.01.2017 00:00:00'",
                string.Empty,
                string.Empty,
                (x) => x == DateTimeOffset.Parse("13.01.2017 00:00:00"),
                (x) => x == DateTimeOffset.Parse("16.01.2017 00:00:00"));

            EvaluateQuery(@"repeat every days where GetDayOfWeek() in (monday, frIday) start at '12.01.2017 00:00:00'",
                string.Empty,
                string.Empty,
                (x) => x == DateTimeOffset.Parse("13.01.2017 00:00:00"),
                (x) => x == DateTimeOffset.Parse("16.01.2017 00:00:00"));
        }

        [TestMethod]
        public void CodeGenerationVisitor_GetSpecificDaysOfMonthThatAlsoAreSpecificDaysOfWeek_ShouldPass()
        {
            EvaluateQuery(@"repeat every days where GetDay() in (21,22) and GetDayOfWeek() in (1,2) start at '13.01.2017 00:00:00'", 
                string.Empty, 
                string.Empty,
                (x) => true,
                (x) => true,
                (x) => true);
        }

        public void EvaluateQuery(string query, string startAt, string stopAt, params Func<DateTimeOffset?, bool>[] funcs)
        {
            EvaluateQuery(query, startAt, stopAt, (DateTimeOffset?)null, funcs);
        }

        public void EvaluateQuery(string query, string startAt, string stopAt, DateTimeOffset? referenceTime = null, params Func<DateTimeOffset?, bool>[] funcs)
        {

            var machine = Parse(string.Format(query, startAt, stopAt));

            if (referenceTime.HasValue)
                machine.ReferenceTime = referenceTime.Value;

            var datetime = default(DateTimeOffset?);
            var index = 0;

            while((datetime = machine.NextFire()).HasValue && index < funcs.Length)
            {
                Assert.IsTrue(funcs[index](datetime));
                index += 1;
            }

            Assert.IsTrue(index == funcs.Length);
        }

        public static bool TestMethodWithDateTimeOffset(DateTimeOffset? date)
        {
            _staticMethod1Called = true;
            return true;
        }

        public static bool TestMethodWithDateTimeOffset(DateTimeOffset? date, long? year)
        {
            _staticMethod2Called = true;
            return true;
        }

        private RdlVirtualMachine Parse(string query)
        {
            var gm = new RdlMetadata();

            var lexer = new LexerComplexTokensDecorator(query);
            var parser = new RdlParser(lexer, gm, TimeZoneInfo.Local.BaseUtcOffset, new string[] {
                "dd/M/yyyy H:m:s",
                "dd/M/yyyy h:m:s tt",
                "dd.M.yyyy H:m:s",
                "dd.M.yyyy h:m:s tt",
                "yyyy-mm.dd HH:mm:ss",
                "yyyy/mm/dd H:m:s",
                "dd.M.yyyy"
            }, new System.Globalization.CultureInfo("en-US"));

            var methods = new DefaultMethods();

            gm.RegisterMethod(nameof(TestMethodWithDateTimeOffset), GetType().GetMethod(nameof(TestMethodWithDateTimeOffset), new[] { typeof(DateTimeOffset?) }));
            gm.RegisterMethod(nameof(TestMethodWithDateTimeOffset), GetType().GetMethod(nameof(TestMethodWithDateTimeOffset), new[] { typeof(DateTimeOffset?), typeof(long?) }));
            gm.RegisterMethod(nameof(DefaultMethods.GetDate), methods.GetType().GetMethod(nameof(DefaultMethods.GetDate), new Type[] { }));
            gm.RegisterMethod(nameof(DefaultMethods.GetYear), methods.GetType().GetMethod(nameof(DefaultMethods.GetYear), new Type[] { }));
            gm.RegisterMethod(nameof(DefaultMethods.GetDay), methods.GetType().GetMethod(nameof(DefaultMethods.GetDay), new Type[] { }));
            gm.RegisterMethod(nameof(DefaultMethods.GetWeekOfMonth), methods.GetType().GetMethod(nameof(DefaultMethods.GetWeekOfMonth), new Type[] { }));
            gm.RegisterMethod(nameof(DefaultMethods.GetDayOfYear), methods.GetType().GetMethod(nameof(DefaultMethods.GetDayOfYear), new Type[0]));
            gm.RegisterMethod(nameof(DefaultMethods.GetDayOfWeek), methods.GetType().GetMethod(nameof(DefaultMethods.GetDayOfWeek), new Type[0]));
            gm.RegisterMethod(nameof(DefaultMethods.GetHour), methods.GetType().GetMethod(nameof(DefaultMethods.GetHour), new Type[0]));
            gm.RegisterMethod(nameof(DefaultMethods.GetMonth), methods.GetType().GetMethod(nameof(DefaultMethods.GetMonth), new Type[0]));
            gm.RegisterMethod(nameof(DefaultMethods.GetMinute), methods.GetType().GetMethod(nameof(DefaultMethods.GetMinute), new Type[0]));
            gm.RegisterMethod(nameof(DefaultMethods.GetSecond), methods.GetType().GetMethod(nameof(DefaultMethods.GetSecond), new Type[0]));

            var visitor = new RdlCodeGenerator(gm);

            var node = parser.ComposeRootComponents();

            var traverseVisitor = new CodeGenerationTraverser(visitor);

            node.Accept(traverseVisitor);

            var machine = visitor.VirtualMachine;
            methods.SetMachine(machine);

            return machine;
        }
    }
}
