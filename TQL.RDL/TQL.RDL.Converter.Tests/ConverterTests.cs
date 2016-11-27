using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TQL.RDL.Converter.Tests
{
    [TestClass]
    public class ConverterTests
    {
        private static bool testACalled = false;
        private static bool testBCalled = false;

        [TestMethod]
        public void Converter_CheckIsMethodRegistered_ShouldPass()
        {
            var methods = new System.Reflection.MethodInfo[2] {
                GetType().GetMethod(nameof(TestA), new Type[] { typeof(DateTimeOffset?) }),
                GetType().GetMethod(nameof(TestB), new Type[] { typeof(DateTimeOffset?) })
            };
            var request = new ConvertionRequest("repeat every 5 seconds where TestA(@current) and TestB(@current) start at '2016/06/06 14:00:00'", DateTimeOffset.Parse("2016/06/06 14:00:00"), false, null, null, methods);

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

        [TestMethod]
        public void Converter_IsConvertible_ShouldBe()
        {
            TestDefaultMethods("repeat every days where GetDay() in (21) and GetMonth() = 5 start at '21.05.1991 00:04:24'", "21.05.1991 00:04:24");
        }

        
        private void TestDefaultMethods(string query, string refTime)
        {
            var request = new ConvertionRequest(query, null);

            RdlTimeline timeline = new RdlTimeline(false);
            var response = timeline.Convert(request);

            var fire = response.Output.NextFire();

            Assert.AreEqual(DateTimeOffset.Parse(refTime), fire.Value);
        }

        public static bool TestA(DateTimeOffset? current)
        {
            testACalled = true;
            return true;
        }

        public static bool TestB(DateTimeOffset? current)
        {
            testBCalled = true;
            return true;
        }
    }
}
