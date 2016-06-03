using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using TQL.RDL.Parser;

namespace TQL.RDL.Evaluator.Tests
{
    [TestClass]
    public class EvaluatorTests
    {
        private static bool staticMethodCalled = false;
        private bool methodCalled = false;

        [Ignore()]
        [TestMethod]
        public void TestMethod1()
        {
            var lexer = new LexerComplexTokensDecorator("repeat every hours where @hour in (21,22,23,24) and 3 = 4 and @year < 2100");
            var parser = new RDLParser(lexer);
            var node = parser.ComposeRootComponents();

            RDLCodeGenerationVisitor visitor = new RDLCodeGenerationVisitor();

            node.Accept(visitor);
            var machine = visitor.VirtualMachine;

            while (true)
            {
                var reftime = machine.NextFire();
            }
        }

        [TestMethod]
        public void CodeGenerationVisitor_ComposeFunctionCall_ShouldPass()
        {
            var manager = new MethodManager();
            manager.RegisterMethod(nameof(TestMethodWithDateTimeOffset), null, this.GetType().GetMethod(nameof(TestMethodWithDateTimeOffset), new[] { typeof(DateTimeOffset) }));
            manager.RegisterMethod(nameof(TestMethodWithDateTimeOffset), this, this.GetType().GetMethod(nameof(TestMethodWithDateTimeOffset), new[] { typeof(DateTimeOffset), typeof(int) }));

            var lexer = new LexerComplexTokensDecorator("repeat every hours where TestMethodWithDateTimeOffset(@current, @year) and TestMethodWithDateTimeOffset(@current)");
            var parser = new RDLParser(lexer);
            var node = parser.ComposeRootComponents();

            RDLCodeGenerationVisitor visitor = new RDLCodeGenerationVisitor(manager);

            node.Accept(visitor);
            var machine = visitor.VirtualMachine;
            
            machine.NextFire();

            Assert.IsTrue(staticMethodCalled);
            Assert.IsTrue(methodCalled);
        }

        [TestMethod]
        public void CodeGenerationVisitor_EvaluateSimpleStartAtStopAt_ShouldPass()
        {
            var lexer = new LexerComplexTokensDecorator("repeat every hours start at '21.05.2012 05:00:00' stop at '21.05.2012 12:00:00'");
            var parser = new RDLParser(lexer);
            var node = parser.ComposeRootComponents();

            RDLCodeGenerationVisitor visitor = new RDLCodeGenerationVisitor();

            node.Accept(visitor);

            var machine = visitor.VirtualMachine;

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
            var lexer = new LexerComplexTokensDecorator("repeat every 2 hours start at '21.05.2012 05:00:00' stop at '21.05.2012 12:00:00'");
            var parser = new RDLParser(lexer);
            var node = parser.ComposeRootComponents();

            RDLCodeGenerationVisitor visitor = new RDLCodeGenerationVisitor();

            node.Accept(visitor);

            var machine = visitor.VirtualMachine;

            var refTime = machine.ReferenceTime;
            var datetime = default(DateTimeOffset?);
            while ((datetime = machine.NextFire()).HasValue)
            {
                Assert.AreEqual(refTime, datetime);
                refTime = refTime.Value.AddHours(2);
            }

            Assert.AreEqual(null, datetime);
        }

        public static bool TestMethodWithDateTimeOffset(DateTimeOffset date)
        {
            staticMethodCalled = true;
            return true;
        }

        public bool TestMethodWithDateTimeOffset(DateTimeOffset date, long year)
        {
            methodCalled = true;
            return true;
        }
    }
}
