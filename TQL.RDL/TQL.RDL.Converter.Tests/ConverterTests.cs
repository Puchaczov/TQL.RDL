using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TQL.RDL.Converter.Tests
{
    [TestClass]
    public class ConverterTests
    {
        private static bool _testACalled;
        private static bool _testBCalled;

        [TestMethod]
        public void Converter_CheckIsMethodRegistered_ShouldPass()
        {
            var methods = new System.Reflection.MethodInfo[2] {
                GetType().GetMethod(nameof(TestA), new[] { typeof(DateTimeOffset?) }),
                GetType().GetMethod(nameof(TestB), new[] { typeof(DateTimeOffset?) })
            };
            var request = new ConvertionRequest("repeat every 5 seconds where TestA(@current) and TestB(@current) start at '06.06.2016 14:00:00'", TimeZoneInfo.Local, TimeZoneInfo.Local, false, null, methods);

            var timeline = new RdlTimeline(false);

            var response = timeline.Convert(request);

            response.Output.NextFire();
            
            Assert.IsTrue(_testACalled);
            Assert.IsTrue(_testBCalled);
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

        [TestMethod]
        public void Converter_SupportCaseWhenQuery_ShouldSupport()
        {
            TestDefaultMethods(@"
            repeat every days where 
                1 = (case 
                        when GetDay() in (22, 23) then 1 
                        else 0 
                    esac) start at '21.12.2016'", "22.12.2016");
        }


        private void TestDefaultMethods(string query, string fireTime)
        {
            var request = new ConvertionRequest(query, TimeZoneInfo.Local, TimeZoneInfo.Local, false, new[] {
                "dd.M.yyyy",
                "dd.M.yyyy hh:mm:ss"
            });

            var timeline = new RdlTimeline(false);
            var response = timeline.Convert(request);

            Assert.IsNotNull(response.Output);

            var fireAt = response.Output.NextFire();

            Assert.AreEqual(DateTimeOffset.Parse(fireTime), fireAt.Value);
        }

        public static bool TestA(DateTimeOffset? current)
        {
            _testACalled = true;
            return true;
        }

        public static bool TestB(DateTimeOffset? current)
        {
            _testBCalled = true;
            return true;
        }
    }
}
