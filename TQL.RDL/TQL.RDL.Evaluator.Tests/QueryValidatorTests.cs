using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TQL.RDL.Parser;

namespace TQL.RDL.Evaluator.Tests
{
    [TestClass]
    public class QueryValidatorTests
    {

        [TestMethod]
        public void QueryValidator_RepeatEveryWithBadDatePartFrac_ShouldPass()
        {
            var validator = Parse("repeat every 4 seecondsds");


            Assert.AreEqual(1, validator.Errors.Count());
            Assert.AreEqual(MessageLevel.Error, validator.Errors.First().Level);
        }

        [TestMethod]
        public void QueryValidator_CheckReturnType_Or_ShouldPass()
        {
            QueryValidator_CheckReturnType("repeat every 1 seconds where GetDate() or 4", 1, SyntaxErrorKind.ImproperType);
            QueryValidator_CheckReturnType("repeat every 1 seconds where 1 = 1 or 2 = 2", 0);
        }

        [TestMethod]
        public void QueryValidator_CheckReturnType_And_ShouldPass()
        {
            QueryValidator_CheckReturnType("repeat every 1 seconds where GetDate() and 4", 1, SyntaxErrorKind.ImproperType);
            QueryValidator_CheckReturnType("repeat every 1 seconds where 1 = 1 and 2 = 2", 0);
        }

        [TestMethod]
        public void QueryValidator_CheckReturnType_Equality_ShouldPass()
        {
            QueryValidator_CheckReturnType("repeat every 1 seconds where GetDate() = 4", 1, SyntaxErrorKind.ImproperType);
            QueryValidator_CheckReturnType("repeat every 1 seconds where 1 = 1 and 2 = 2", 0);
        }

        private void QueryValidator_CheckReturnType(string query, int errorCount, params SyntaxErrorKind[] errorKinds)
        {
            //Binary nodes have to operate on the same types.
            var validator = Parse(query);


            Assert.AreEqual(errorCount, errorKinds.Length);
            Assert.AreEqual(errorCount, validator.Errors.Count());

            for(int i = 0; i < errorCount; ++i)
            {
                Assert.AreEqual(errorKinds[i], (validator.Errors.ElementAt(i) as SyntaxError).Kind);
            }
        }

        public RDLQueryValidator Parse(string query)
        {
            var lexer = new LexerComplexTokensDecorator(query);
            var parser = new RDLParser(lexer);
            var node = parser.ComposeRootComponents();

            var validator = new RDLQueryValidator();
            var methods = new DefaultMethods();

            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.GetDate), methods.GetType().GetMethod(nameof(DefaultMethods.GetDate), new Type[] { }));
            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.GetYear), methods.GetType().GetMethod(nameof(DefaultMethods.GetYear), new Type[] { }));
            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.GetDay), methods.GetType().GetMethod(nameof(DefaultMethods.GetDay), new Type[] { }));

            node.Accept(validator);

            return validator;
        }
    }
}
