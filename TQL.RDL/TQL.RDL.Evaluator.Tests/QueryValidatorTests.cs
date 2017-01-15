using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TQL.RDL.Evaluator.Visitors;
using TQL.RDL.Parser;

namespace TQL.RDL.Evaluator.Tests
{
    [TestClass]
    public class QueryValidatorTests
    {
        [TestMethod]
        public void QueryValidator_RepeatEveryWithBadDatePartFrac_ShouldPass()
        {
            var validator = Parse("repeat every 4 seecondsds start at '21.04.2013'");


            Assert.AreEqual(1, validator.Errors.Count());
            Assert.AreEqual(MessageLevel.Error, validator.Errors.First().Level);
        }

        [TestMethod]
        public void QueryValidator_CheckReturnType_Or_ShouldPass()
        {
            QueryValidator_CheckReturnType("repeat every 1 seconds where GetDate() or 4 start at '21.04.2013'", 1, SyntaxErrorKind.ImproperType);
            QueryValidator_CheckReturnType("repeat every 1 seconds where 1 = 1 or 2 = 2 start at '21.04.2013'", 0);
        }

        [TestMethod]
        public void QueryValidator_CheckReturnType_And_ShouldPass()
        {
            QueryValidator_CheckReturnType("repeat every 1 seconds where GetDate() and 4 start at '21.04.2013'", 1, SyntaxErrorKind.ImproperType);
            QueryValidator_CheckReturnType("repeat every 1 seconds where 1 = 1 and 2 = 2 start at '21.04.2013'", 0);
        }

        [TestMethod]
        public void QueryValidator_CheckReturnType_Equality_ShouldPass()
        {
            QueryValidator_CheckReturnType("repeat every 1 seconds where GetDate() = 4 start at '21.04.2013'", 1, SyntaxErrorKind.ImproperType);
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
            QueryValidator_CheckReturnType("repeat every days", 1,  SyntaxErrorKind.MissingValue);
        }

        private void QueryValidator_CheckReturnType(string query, int errorCount, params SyntaxErrorKind[] errorKinds)
        {
            //Binary nodes have to operate on the same types.
            var validator = Parse(query);


            Assert.AreEqual(errorCount, errorKinds.Length);
            Assert.AreEqual(errorCount, validator.Errors.Count());

            for(var i = 0; i < errorCount; ++i)
            {
                Assert.AreEqual(errorKinds[i], (validator.Errors.ElementAt(i) as SyntaxError).Kind);
            }
        }

        public RdlQueryValidator Parse(string query)
        {
            var gm = new RdlMetadata();
            var lexer = new LexerComplexTokensDecorator(query);
            var parser = new RdlParser(lexer, gm, TimeZoneInfo.Local.BaseUtcOffset, new string[0], new System.Globalization.CultureInfo("en-US"));
            var node = parser.ComposeRootComponents();

            var methods = new DefaultMethods();

            gm.RegisterMethod(nameof(DefaultMethods.GetDate), methods.GetType().GetMethod(nameof(DefaultMethods.GetDate), new Type[] { }));
            gm.RegisterMethod(nameof(DefaultMethods.GetYear), methods.GetType().GetMethod(nameof(DefaultMethods.GetYear), new Type[] { }));
            gm.RegisterMethod(nameof(DefaultMethods.GetDay), methods.GetType().GetMethod(nameof(DefaultMethods.GetDay), new Type[] { }));
            
            var validator = new RdlQueryValidator(gm);
            var traverser = new CodeGenerationTraverser(validator);
            node.Accept(traverser);

            return validator;
        }
    }
}
