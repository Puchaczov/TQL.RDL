using Microsoft.VisualStudio.TestTools.UnitTesting;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Tests
{
    [TestClass]
    public class RDLWhereParserTests
    {
        [TestMethod]
        public void WhereConditions_ParseComplexQuery()
        {
            var tokens = Parse("GetDay() in (21,22,23,24) and 3 = 4 and GetYear() < 2100");

            Assert.AreEqual(SyntaxType.VarArg, tokens[0].TokenType);
            Assert.AreEqual(SyntaxType.Function, tokens[1].TokenType);
            Assert.AreEqual(SyntaxType.Numeric, tokens[2].TokenType);
            Assert.AreEqual(SyntaxType.Numeric, tokens[3].TokenType);
            Assert.AreEqual(SyntaxType.Numeric, tokens[4].TokenType);
            Assert.AreEqual(SyntaxType.Numeric, tokens[5].TokenType);
            Assert.AreEqual(SyntaxType.VarArg, tokens[6].TokenType);
            Assert.AreEqual(SyntaxType.In, tokens[7].TokenType);
            Assert.AreEqual(SyntaxType.Numeric, tokens[8].TokenType);
            Assert.AreEqual(SyntaxType.Numeric, tokens[9].TokenType);
            Assert.AreEqual(SyntaxType.Equality, tokens[10].TokenType);
            Assert.AreEqual(SyntaxType.And, tokens[11].TokenType);
            Assert.AreEqual(SyntaxType.VarArg, tokens[12].TokenType);
            Assert.AreEqual(SyntaxType.Function, tokens[13].TokenType);
            Assert.AreEqual(SyntaxType.Numeric, tokens[14].TokenType);
            Assert.AreEqual(SyntaxType.Less, tokens[15].TokenType);
            Assert.AreEqual(SyntaxType.And, tokens[16].TokenType);
        }

        private static Token[] Parse(string query)
        {
            RDLWhereParser parser = new RDLWhereParser();
            LexerComplexTokensDecorator dec = new LexerComplexTokensDecorator(query);
            return parser.Parse(dec);
        }
    }
}
