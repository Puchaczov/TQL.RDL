﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TQL.RDL.Converter.Tests
{
    [TestClass]
    public class ConverterTests
    {
        private static bool testACalled = false;
        private bool testBCalled = false;

        [TestMethod]
        public void Converter_CheckIsMethodRegistered_ShouldPass()
        {
            var methods = new System.Collections.Generic.KeyValuePair<object, System.Reflection.MethodInfo>[2] {
                new System.Collections.Generic.KeyValuePair<object, System.Reflection.MethodInfo>(null, GetType().GetMethod(nameof(TestA), new Type[] { typeof(DateTimeOffset) })),
                new System.Collections.Generic.KeyValuePair<object, System.Reflection.MethodInfo>(this, GetType().GetMethod(nameof(TestB), new Type[] { typeof(DateTimeOffset) }))
            };
            var request = new ConvertionRequest("repeat every 5 seconds where TestA(@current) and TestB(@current)", DateTimeOffset.UtcNow, methods);

            RdlTimeline timeline = new RdlTimeline(false);

            var response = timeline.Convert(request);

            response.Output.NextFire();
            
            Assert.IsTrue(testACalled);
            Assert.IsTrue(testBCalled);
        }

        [TestMethod]
        public void Converter_DefaultMethods_IsLastDayOfMonth_ShouldPass()
        {
            TestDefaultMethods("repeat every days where IsLastDayOfMonth() start at '30.05.2016'", "31.05.2016");
            TestDefaultMethods("repeat every days where IsLastDayOfMonth(@current) start at '30.05.2016'", "31.05.2016");
        }

        [TestMethod]
        public void Converter_DefaultMethods_IsDayOfWeek_ShouldPass()
        {
            TestDefaultMethods("repeat every days where IsDayOfWeek(GetDate(), 2) start at '30.05.2016'", "31.05.2016");
            TestDefaultMethods("repeat every days where IsDayOfWeek(2) start at '30.05.2016'", "31.05.2016");
        }

        [TestMethod]
        public void Converter_DefaultMethods_IsOdd_ShouldPass()
        {
            TestDefaultMethods("repeat every days where IsOdd(@day) start at '30.05.2016'", "31.05.2016");
        }

        [TestMethod]
        public void Converter_DefaultMethods_IsWorkingDay_ShouldPass()
        {
            TestDefaultMethods("repeat every days where IsWorkingDay() start at '29.05.2016'", "30.05.2016");
        }

        
        private void TestDefaultMethods(string query, string refTime)
        {
            var request = new ConvertionRequest(query, null);

            RdlTimeline timeline = new RdlTimeline(false);
            var response = timeline.Convert(request);

            var fire = response.Output.NextFire();

            Assert.AreEqual(DateTimeOffset.Parse(refTime), fire.Value);
        }

        public static bool TestA(DateTimeOffset current)
        {
            testACalled = true;
            return true;
        }

        public bool TestB(DateTimeOffset current)
        {
            testBCalled = true;
            return true;
        }
    }
}
