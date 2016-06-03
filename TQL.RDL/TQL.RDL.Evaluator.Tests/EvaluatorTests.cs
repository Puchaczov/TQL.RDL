using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using TQL.RDL.Parser;

namespace TQL.RDL.Evaluator.Tests
{
    [TestClass]
    public class UnitTest1
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
