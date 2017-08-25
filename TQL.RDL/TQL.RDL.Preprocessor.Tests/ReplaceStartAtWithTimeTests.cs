using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace TQL.RDL.Preprocessor.Tests
{
    [TestClass]
    public class ReplaceStartAtWithTimeTests
    {
        private readonly string[] _defaultFormats =
        {
            "dd/M/yyyy H:m:s",
            "dd/M/yyyy h:m:s tt",
            "dd.M.yyyy H:m:s",
            "dd.M.yyyy h:m:s tt"
        };

        private class PublicReplaceStartWithAtTime : ReplaceStartAtWithTime
        {
            public PublicReplaceStartWithAtTime(DateTimeOffset time, string[] formats)
                : base(time, formats, Strings.StartAtDateExtractRegex)
            { }

            [DebuggerStepThrough]
            public new string Process(string input)
            {
                return base.Process(input);
            }
        }

        [TestMethod]
        public void Preprocessor_FillStartAtWithDateTimeValues_ShouldPass()
        {
            var time = DateTimeOffset.Parse("24.08.2017 00:00:00");
            var s = new PublicReplaceStartWithAtTime(time, _defaultFormats);

            var finalString = $"start at '{time.ToString("dd.MM.yyyy HH:mm:ss")}'";

            Assert.AreEqual(finalString, s.Process("start at 'dd.08.2017 00:00:00'"));
            Assert.AreEqual(finalString, s.Process("start at '24.MM.2017 00:00:00'"));
            Assert.AreEqual(finalString, s.Process("start at '24.08.yyyy 00:00:00'"));
            Assert.AreEqual(finalString, s.Process("start at '24.08.2017 hh:00:00'"));
            Assert.AreEqual(finalString, s.Process("start at '24.08.2017 00:mm:00'"));
            Assert.AreEqual(finalString, s.Process("start at '24.08.2017 00:00:ss'"));
            Assert.AreEqual(finalString, s.Process("start at 'dd.MM.yyyy hh:mm:ss'"));
        }
    }
}
