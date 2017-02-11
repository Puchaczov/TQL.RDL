using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TQL.RDL.Evaluator.Attributes;

namespace TQL.RDL.Converter.Tests
{
    [TestClass]
    [BindableClass]
    public class ConverterTests
    {
        private static bool _testACalled;
        private static bool _testBCalled;

        [TestMethod]
        public void Converter_CheckIsMethodRegistered_ShouldPass()
        {
            var request =
                new ConvertionRequest<ConverterTests>(
                    "repeat every 5 seconds where TestA(@current) and TestB(@current) start at '06.06.2016 14:00:00'",
                    TimeZoneInfo.Local, TimeZoneInfo.Local, false, null);

            var timeline = new RdlTimeline<ConverterTests>(false);

            var response = timeline.Convert(request);

            response.Output.NextFire();

            Assert.IsTrue(_testACalled);
            Assert.IsTrue(_testBCalled);
        }

        [TestMethod]
        public void Converter_DefaultMethods_IsLastDayOfMonth_ShouldPass()
        {
            TestDefaultMethods("repeat every days where IsLastDayOfMonth() start at '30.05.2016'", "31.05.2016");
        }

        [TestMethod]
        public void Converter_DefaultMethods_IsDayOfWeek_ShouldPass()
        {
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
            TestDefaultMethods(
                "repeat every days where GetDay() in (21) and GetMonth() = 5 start at '21.05.1991 00:04:24'",
                "21.05.1991 00:04:24");
        }

        [TestMethod]
        public void Converter_CallingConventionTest_ShouldPass()
        {
            TestMethods<ConverterTests>(
                "repeat every days where TestC('adsad', 'qeqwew', 256, 4) and TestC('adsad', 'qeqwew', 256, 4) start at '21.05.1991 00:04:24'",
                "21.05.1991 00:04:24");
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
            => TestMethods<DefaultMethodsAggregator>(query, fireTime);

        private void TestMethods<TMethods>(string query, string fireTime) where TMethods : new()
        {
            var request = new ConvertionRequest<TMethods>(query, TimeZoneInfo.Local, TimeZoneInfo.Local, false, new[]
            {
                "dd.M.yyyy",
                "dd.M.yyyy hh:mm:ss"
            });

            var timeline = new RdlTimeline<TMethods>(false);
            var response = timeline.Convert(request);

            Assert.IsNotNull(response.Output);

            var fireAt = response.Output.NextFire();

            Assert.AreEqual(DateTimeOffset.Parse(fireTime), fireAt.Value);
        }

        [BindableMethod]
        public static bool TestA(DateTimeOffset? current)
        {
            _testACalled = true;
            return true;
        }

        [BindableMethod]
        public static bool TestB(DateTimeOffset? current)
        {
            _testBCalled = true;
            return true;
        }

        [BindableMethod]
        public static bool TestC([InjectReferenceTime] DateTimeOffset date, string a, string b, long c, long d) => true;
    }
}