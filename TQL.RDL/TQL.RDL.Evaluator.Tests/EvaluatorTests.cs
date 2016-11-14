using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TQL.RDL.Parser;

namespace TQL.RDL.Evaluator.Tests
{
    [TestClass]
    public class EvaluatorTests
    {
        private static bool staticMethod1Called = false;
        private static bool staticMethod2Called = false;
        

        [TestMethod]
        public void CodeGenerationVisitor_WithAlwaysFalseNode_ShouldReturnNull()
        {
            var machine = Parse("repeat every days where GetDay() in (21,22,23,24) and 3 = 4 and GetYear() < 2100 start at '2000/01/01' stop at '2100/05/05'");

            machine.ReferenceTime = DateTimeOffset.Parse("2000/01/01");
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
            var machine = Parse("repeat every hours where TestMethodWithDateTimeOffset(GetDate(), GetYear()) and TestMethodWithDateTimeOffset(GetDate()) start at '2016-06-07 22:00:00'");
            machine.ReferenceTime = new DateTimeOffset(2016, 6, 7, 22, 0, 0, new TimeSpan());

            machine.NextFire();

            Assert.IsTrue(staticMethod1Called);
            Assert.IsTrue(staticMethod2Called);
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
                refTime = refTime.Value.AddHours(1);
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
                refTime = refTime.Value.AddHours(2);
            }

            Assert.AreEqual(null, datetime);
        }

        [TestMethod]
        public void CodeGenerationVisitor_CaseWhen_ShouldPass()
        {
            EvaluateQuery("repeat every 1 days where 1 = (case when (GetDay() in (1,5)) then (1) when (GetDay() in (10,15)) then (1) else (0) esac) start at {0} stop at {1}", "'01.01.2012 00:00:00'", "'01.02.2012 00:00:00'",
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
        public void CodeGenerationVisitor_EvaluateNullWhenStopAtReached_ShouldReturnNull()
        {
            var machine = Parse("repeat every 2 hours start at '21.05.2012 13:00:00' stop at '21.05.2012 12:00:00'");

            Assert.AreEqual(null, machine.NextFire());
            Assert.AreEqual(null, machine.NextFire());
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluateBasicOperators_ShouldPass()
        {
            EvaluateQuery("repeat every hours where 1 + 2 = 3 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.05.2012 15:00'", 
                (x) => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every hours where 5 - 4 = 1 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.05.2012 15:00'",
                (x) => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every hours where 5 * 4 = 19 + 1 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.05.2012 15:00'",
                (x) => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every hours where 20 / 2 = 50 / 5 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.05.2012 15:00'",
                (x) => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every hours where 20 + (2 * 5) - 4 + 1 = 20 + 6 + 1 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.05.2012 15:00'",
                (x) => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 14:00:00"),
                (x) => x == DateTimeOffset.Parse("21.05.2012 15:00:00"));

            EvaluateQuery("repeat every days where GetDay() in (22,25,27) start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'31.05.2012 15:00'",
                (x) => x == DateTimeOffset.Parse("22.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("25.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("27.05.2012 13:00:00"));

            EvaluateQuery("repeat every days where GetDay() not in (22,25,27) start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'31.05.2012 15:00'",
                (x) => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("23.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("24.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("26.05.2012 13:00:00"));

            EvaluateQuery("repeat every days where GetDay() not in (22,25,27) start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'31.05.2012 15:00'",
                (x) => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("23.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("24.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("26.05.2012 13:00:00"));

            EvaluateQuery("repeat every days where GetDay() * GetYear() = 29 * 2012 start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'31.05.2012 15:00'",
                (x) => x == DateTimeOffset.Parse("29.05.2012 13:00:00"));

            EvaluateQuery("repeat every 2 days start at {0} stop at {1}", "'21.05.2012 13:00:00'", "'21.06.2012 23:00:00'",
                (x) => x == DateTimeOffset.Parse("21.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("23.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("25.05.2012 13:00:00"),
                (x) => x == DateTimeOffset.Parse("27.05.2012 13:00:00"));
        }

        public void EvaluateQuery(string query, string startAt, string stopAt, params Func<DateTimeOffset?, bool>[] funcs)
        {

            var machine = Parse(string.Format(query, startAt, stopAt));

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
            staticMethod1Called = true;
            return true;
        }

        public static bool TestMethodWithDateTimeOffset(DateTimeOffset? date, long? year)
        {
            staticMethod2Called = true;
            return true;
        }

        public RDLVirtualMachine Parse(string query)
        {
            var lexer = new LexerComplexTokensDecorator(query);
            var parser = new RDLParser(lexer);
            var node = parser.ComposeRootComponents();

            var visitor = new RDLCodeGenerator();
            var methods = new DefaultMethods();

            GlobalMetadata.RegisterMethod(nameof(TestMethodWithDateTimeOffset), this.GetType().GetMethod(nameof(TestMethodWithDateTimeOffset), new[] { typeof(DateTimeOffset?) }));
            GlobalMetadata.RegisterMethod(nameof(TestMethodWithDateTimeOffset), this.GetType().GetMethod(nameof(TestMethodWithDateTimeOffset), new[] { typeof(DateTimeOffset?), typeof(long?) }));
            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.GetDate), methods.GetType().GetMethod(nameof(DefaultMethods.GetDate), new Type[] { }));
            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.GetYear), methods.GetType().GetMethod(nameof(DefaultMethods.GetYear), new Type[] { }));
            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.GetDay), methods.GetType().GetMethod(nameof(DefaultMethods.GetDay), new Type[] { }));

            node.Accept(visitor);

            var machine = visitor.VirtualMachine;
            methods.SetMachine(machine);

            return machine;
        }
    }
}
