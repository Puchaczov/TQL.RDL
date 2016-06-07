﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var machine = Parse("repeat every hours where @hour in (21,22,23,24) and 3 = 4 and @year < 2100");

            while (true)
            {
                var reftime = machine.NextFire();
            }
        }

        [TestMethod]
        public void CodeGenerationVisitor_ComposeFunctionCall_ShouldPass()
        {
            var machine = Parse("repeat every hours where TestMethodWithDateTimeOffset(GetDate(), GetYear()) and TestMethodWithDateTimeOffset(GetDate())");

            machine.ReferenceTime = new DateTimeOffset(2016, 6, 7, 22, 0, 0, new TimeSpan());

            machine.NextFire();

            Assert.IsTrue(staticMethodCalled);
            Assert.IsTrue(methodCalled);
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
        public void CodeGenerationVisitor_EvaluateNullWhenStopAtReached_ShouldReturnNull()
        {
            var machine = Parse("repeat every 2 hours start at '21.05.2012 13:00:00' stop at '21.05.2012 12:00:00'");

            Assert.AreEqual(null, machine.NextFire());
            Assert.AreEqual(null, machine.NextFire());
        }

        public static bool TestMethodWithDateTimeOffset(DateTimeOffset? date)
        {
            staticMethodCalled = true;
            return true;
        }

        public bool TestMethodWithDateTimeOffset(DateTimeOffset? date, long? year)
        {
            methodCalled = true;
            return true;
        }

        public RDLVirtualMachine Parse(string query)
        {
            var lexer = new LexerComplexTokensDecorator(query);
            var parser = new RDLParser(lexer);
            var node = parser.ComposeRootComponents();

            RDLCodeGenerationVisitor visitor;
            visitor = new RDLCodeGenerationVisitor();

            DefaultMethods methods = new DefaultMethods();

            GlobalMetadata.RegisterMethod(nameof(TestMethodWithDateTimeOffset), null, this.GetType().GetMethod(nameof(TestMethodWithDateTimeOffset), new[] { typeof(DateTimeOffset?) }));
            GlobalMetadata.RegisterMethod(nameof(TestMethodWithDateTimeOffset), this, this.GetType().GetMethod(nameof(TestMethodWithDateTimeOffset), new[] { typeof(DateTimeOffset?), typeof(long?) }));
            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.GetDate), methods, methods.GetType().GetMethod(nameof(DefaultMethods.GetDate), new Type[] { }));
            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.GetYear), methods, methods.GetType().GetMethod(nameof(DefaultMethods.GetYear), new Type[] { }));

            node.Accept(visitor);

            var machine = visitor.VirtualMachine;

            methods.SetMachine(machine);

            return machine;
        }
    }
}
