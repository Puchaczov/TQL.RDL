using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TQL.Interfaces;
using TQL.RDL.Evaluator.Attributes;

namespace TQL.RDL.Converter.Tests
{
    [TestClass]
    public class RegisterAndCallCustomFunctionTests
    {
        private class CustomMethodsAggregator : DefaultMethodsAggregator
        {
            private int _calledTimes;

            [BindableMethod]
            public bool TestCall()
            {
                return true;
            }

            [BindableMethod]
            public bool TestCall(int param)
            {
                Assert.AreEqual(4, param);
                return true;
            }

            [BindableMethod]
            public bool TestCallWithReferenceTime([InjectReferenceTime] DateTimeOffset referenceTime)
            {
                return true;
            }

            [BindableMethod]
            public bool TestCallWithLastFire([InjectLastFire] DateTimeOffset? lastFireTime)
            {
                _calledTimes += 1;
                if (_calledTimes <= 1)
                {
                    Assert.IsNull(lastFireTime);
                }
                else
                {
                    Assert.IsNotNull(lastFireTime);
                }
                return true;
            }

            [BindableMethod]
            public bool TestCallMixedBoth([InjectLastFire] DateTimeOffset? lastFireTime,
                [InjectReferenceTime] DateTimeOffset referenceTime)
            {
                _calledTimes += 1;
                if (_calledTimes <= 1)
                {
                    Assert.IsNull(lastFireTime);
                }
                else
                {
                    Assert.IsNotNull(lastFireTime);
                }
                return true;
            }

            [BindableMethod]
            public bool TestCallWithStartAtInjected([InjectStartAt] DateTimeOffset startAt, string startAtToParse)
            {
                Assert.AreEqual(DateTimeOffset.Parse(startAtToParse), startAt);
                return true;
            }

            [BindableMethod]
            public bool TestCallWithStopAtInjected([InjectStopAt] DateTimeOffset? stopAt, string stopAtToParse)
            {
                Assert.AreEqual(DateTimeOffset.Parse(stopAtToParse), stopAt);
                return true;
            }

            [BindableMethod]
            public bool TestCallWithStopAtNullInjected([InjectStopAt] DateTimeOffset? stopAt)
            {
                Assert.IsNull(stopAt);
                return true;
            }
        }

        [TestMethod]
        public void TestCustomFunction()
        {
            var evaluator = ToEvaluator("repeat every seconds where TestCall() start at '21.04.2017'");
            Assert.AreEqual(DateTimeOffset.Parse("21.04.2017"), evaluator.NextFire());
        }

        [TestMethod]
        public void TestCustomFunctionWithParameter()
        {
            var evaluator = ToEvaluator("repeat every seconds where TestCall(4) start at '21.04.2017'");
            Assert.AreEqual(DateTimeOffset.Parse("21.04.2017"), evaluator.NextFire());
        }

        [TestMethod]
        public void TestCustomFunctionWithInjectedReferenceTime()
        {
            var evaluator = ToEvaluator("repeat every seconds where TestCallWithReferenceTime() start at '21.04.2017'");
            Assert.AreEqual(DateTimeOffset.Parse("21.04.2017"), evaluator.NextFire());
        }

        [TestMethod]
        public void TestCustomFunctionWithInjectedLastFire()
        {
            var evaluator = ToEvaluator("repeat every seconds where TestCallWithReferenceTime() start at '21.04.2017'");
            evaluator.NextFire();
            evaluator.NextFire();
        }

        [TestMethod]
        public void TestCustomFunctionWithMixedBoth()
        {
            var evaluator = ToEvaluator("repeat every seconds where TestCallMixedBoth() start at '21.04.2017'");
            Assert.AreEqual(DateTimeOffset.Parse("21.04.2017 00:00:00"), evaluator.NextFire());
            Assert.AreEqual(DateTimeOffset.Parse("21.04.2017 00:00:01"), evaluator.NextFire());
        }

        [TestMethod]
        public void TestCustomFunctionWithInjectedStartAt()
        {
            var evaluator = ToEvaluator("repeat every seconds where TestCallWithStartAtInjected('21.04.2017 00:00:00 +00') start at '21.04.2017'");
            evaluator.NextFire();
        }

        [TestMethod]
        public void TestCustomFunctionWithInjectedStopAt()
        {
            var evaluator = ToEvaluator("repeat every seconds where TestCallWithStopAtInjected('22.04.2017 00:00:00 +00') start at '21.04.2017' stop at '22.04.2017'");
            evaluator.NextFire();
        }

        [TestMethod]
        public void TestCustomFunctionWithInjectedStopAtNull()
        {
            var evaluator = ToEvaluator("repeat every seconds where TestCallWithStopAtNullInjected() start at '21.04.2017'");
            evaluator.NextFire();
        }

        private static IFireTimeEvaluator ToEvaluator(string query)
        {
            var response = TestHelper.Convert<CustomMethodsAggregator>(query, TimeZoneInfo.Local);
            var evaluator = response.Output;

            return evaluator;
        }
    }
}
