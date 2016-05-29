using Microsoft.VisualStudio.TestTools.UnitTesting;
using TQL.RDL.Parser;

namespace TQL.RDL.Evaluator.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var lexer = new LexerComplexTokensDecorator("repeat every hours where @hour in (21,22,23,24) and 3 = 4 and @year < 2100");
            var parser = new RDLParser(lexer);
            var node = parser.ComposeRootComponents();

            RDLCodeGenerationVisitor visitor = new RDLCodeGenerationVisitor();

            node.Accept(visitor);
            var machine = visitor.VirtualMachine;

            while (true)
            {
                var reftime = machine.NextFire();
            }
        }
    }
}
