using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TQL.RDL.Evaluator.Helpers;

namespace TQL.RDL.Evaluator.Tests
{
    [TestClass]
    public class DateTimeOffsetHelperTests
    {
        [TestMethod]
        public void TestWeekOfMonth_ShouldPass()
        {
            Assert.AreEqual(1, DateTimeOffset.Parse("01.01.2017").WeekOfMonth());
            Assert.AreEqual(1, DateTimeOffset.Parse("07.01.2017").WeekOfMonth());
            Assert.AreEqual(2, DateTimeOffset.Parse("08.01.2017").WeekOfMonth());
            Assert.AreEqual(2, DateTimeOffset.Parse("14.01.2017").WeekOfMonth());
            Assert.AreEqual(3, DateTimeOffset.Parse("21.01.2017").WeekOfMonth());
            Assert.AreEqual(4, DateTimeOffset.Parse("22.01.2017").WeekOfMonth());
            Assert.AreEqual(4, DateTimeOffset.Parse("27.01.2017").WeekOfMonth());
            Assert.AreEqual(4, DateTimeOffset.Parse("28.01.2017").WeekOfMonth());
            Assert.AreEqual(5, DateTimeOffset.Parse("29.01.2017").WeekOfMonth());
        }

        [TestMethod]
        public void TestWeekOfMonth_WithStartWeekDay()
        {
            Assert.AreEqual(1, DateTimeOffset.Parse("01.02.2017").WeekOfMonth(DayOfWeek.Sunday));
            Assert.AreEqual(2, DateTimeOffset.Parse("05.02.2017").WeekOfMonth(DayOfWeek.Sunday));
            Assert.AreEqual(3, DateTimeOffset.Parse("12.02.2017").WeekOfMonth(DayOfWeek.Sunday));
            Assert.AreEqual(4, DateTimeOffset.Parse("20.02.2017").WeekOfMonth(DayOfWeek.Sunday));
            Assert.AreEqual(5, DateTimeOffset.Parse("28.02.2017").WeekOfMonth(DayOfWeek.Sunday));
        }
    }
}