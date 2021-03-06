﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TQL.RDL.Evaluator.ErrorHandling;

namespace TQL.RDL.Converter.Tests
{
    [TestClass]
    public class QueryValidatorTests
    {
        [TestMethod]
        public void QueryValidator_RepeatEveryWithBadDatePartFrac_ShouldPass()
        {
            var validator = TestHelper.Convert("repeat every 4 seecondsds start at '21.04.2013'");

            Assert.AreEqual(1, validator.Messages.Count);
            Assert.AreEqual(MessageLevel.Error, validator.Messages.First().Level);
        }

        [TestMethod]
        public void QueryValidator_CheckReturnType_Or_ShouldPass()
        {
            QueryValidator_CheckReturnType("repeat every 1 seconds where Now() or 4 start at '21.04.2013'", 1,
                SyntaxErrorKind.ImproperType);
            QueryValidator_CheckReturnType("repeat every 1 seconds where 1 = 1 or 2 = 2 start at '21.04.2013'", 0);
        }

        [TestMethod]
        public void QueryValidator_CheckReturnType_And_ShouldPass()
        {
            QueryValidator_CheckReturnType("repeat every 1 seconds where Now() and 4 start at '21.04.2013'", 1,
                SyntaxErrorKind.ImproperType);
            QueryValidator_CheckReturnType("repeat every 1 seconds where 1 = 1 and 2 = 2 start at '21.04.2013'", 0);
        }

        [TestMethod]
        public void QueryValidator_CheckReturnType_Equality_ShouldPass()
        {
            QueryValidator_CheckReturnType("repeat every 1 seconds where Now() = 4 start at '21.04.2013'", 1,
                SyntaxErrorKind.ImproperType);
            QueryValidator_CheckReturnType("repeat every 1 seconds where 1 = 1 and 2 = 2 start at '21.04.2013'", 0);
        }

        [TestMethod]
        public void QueryValidator_CheckInOperator_ShouldPass()
        {
            QueryValidator_CheckReturnType("repeat every days where GetDay() in (21) start at '21.04.2013'", 0);
        }

        [TestMethod]
        public void QueryValidator_RequiredStartAtNotAppeared_ShouldPass()
        {
            QueryValidator_CheckReturnType("repeat every days", 1, SyntaxErrorKind.MissingValue);
        }

        private void QueryValidator_CheckReturnType(string query, int errorCount, params SyntaxErrorKind[] errorKinds)
        {
            //Binary nodes have to operate on the same types.
            var validator = TestHelper.Convert(query);

            Assert.AreEqual(errorCount, errorKinds.Length);
            Assert.AreEqual(errorCount, validator.Messages.Count);

            for (var i = 0; i < errorCount; ++i)
                Assert.AreEqual(errorKinds[i], (validator.Messages.ElementAt(i) as SyntaxError).Kind);
        }
    }
}