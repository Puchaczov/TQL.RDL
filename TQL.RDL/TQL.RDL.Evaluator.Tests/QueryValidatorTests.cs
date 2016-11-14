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
