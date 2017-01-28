using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDL.Parser;
using RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Tests
{
    [TestClass]
    public class WhereParserTests
    {
        [TestMethod]
        public void WhereConditions_ParseComplexQuery()
        {
            var tokens = Parse("GetDay() in (21,22,23,24) and 3 = 4 and GetYear() < 2100");

            Assert.AreEqual(StatementType.VarArg, tokens[0].TokenType);
            Assert.AreEqual(StatementType.Function, tokens[1].TokenType);
            Assert.AreEqual(StatementType.Numeric, tokens[2].TokenType);
            Assert.AreEqual(StatementType.Numeric, tokens[3].TokenType);
            Assert.AreEqual(StatementType.Numeric, tokens[4].TokenType);
            Assert.AreEqual(StatementType.Numeric, tokens[5].TokenType);
            Assert.AreEqual(StatementType.VarArg, tokens[6].TokenType);
            Assert.AreEqual(StatementType.In, tokens[7].TokenType);
            Assert.AreEqual(StatementType.Numeric, tokens[8].TokenType);
            Assert.AreEqual(StatementType.Numeric, tokens[9].TokenType);
            Assert.AreEqual(StatementType.Equality, tokens[10].TokenType);
            Assert.AreEqual(StatementType.And, tokens[11].TokenType);
            Assert.AreEqual(StatementType.VarArg, tokens[12].TokenType);
            Assert.AreEqual(StatementType.Function, tokens[13].TokenType);
            Assert.AreEqual(StatementType.Numeric, tokens[14].TokenType);
            Assert.AreEqual(StatementType.Less, tokens[15].TokenType);
            Assert.AreEqual(StatementType.And, tokens[16].TokenType);
        }

        [TestMethod]
        public void WhereConditions_ParseCaseWhenQuery()
        {
            var tokens = Parse("1 > (case when (1) then 5 else 4 esac) or 1 > 2");

            Assert.AreEqual(StatementType.Numeric, tokens[0].TokenType);
            Assert.AreEqual(StatementType.CaseWhenEsac, tokens[1].TokenType);
            Assert.AreEqual(StatementType.Greater, tokens[2].TokenType);
            Assert.AreEqual(StatementType.Numeric, tokens[3].TokenType);
            Assert.AreEqual(StatementType.Numeric, tokens[4].TokenType);
            Assert.AreEqual(StatementType.Greater, tokens[5].TokenType);
            Assert.AreEqual(StatementType.Or, tokens[6].TokenType);
        }

        [TestMethod]
        public void WhereConditions_ParseCaseWhenWithComplexWhen()
        {
            var tokens = Parse("1 > (case when (GetDay() <> (3 + 2)) then 5 else 4 esac)");

            Assert.AreEqual(StatementType.Numeric, tokens[0].TokenType);
            Assert.AreEqual(StatementType.CaseWhenEsac, tokens[1].TokenType);
            Assert.AreEqual(StatementType.Greater, tokens[2].TokenType);
        }

        [TestMethod]
        public void WhereConditions_ParseCaseWhenWithMultipleWhen_ShouldPass()
        {
            var tokens = Parse("1 > case when (1) then (2) when (3) then (4) else (5) esac");

            Assert.AreEqual(StatementType.Numeric, tokens[0].TokenType);
            Assert.AreEqual(StatementType.CaseWhenEsac, tokens[1].TokenType);
            Assert.AreEqual(StatementType.Greater, tokens[2].TokenType);
        }

        [TestMethod]
        public void WhereConditions_ParseAithmeticOperatorsWithoutBrackets_ShouldPass()
        {
            var tokens = Parse("1 = 2 or 3 = 4");

            Assert.AreEqual(StatementType.Numeric, tokens[0].TokenType);
            Assert.AreEqual(StatementType.Numeric, tokens[1].TokenType);
            Assert.AreEqual(StatementType.Equality, tokens[2].TokenType);
            Assert.AreEqual(StatementType.Numeric, tokens[3].TokenType);
            Assert.AreEqual(StatementType.Numeric, tokens[4].TokenType);
            Assert.AreEqual(StatementType.Equality, tokens[5].TokenType);
            Assert.AreEqual(StatementType.Or, tokens[6].TokenType);
        }

        [TestMethod]
        public void WhereConditions_ParseArithmeticWithWord_ShouldPass()
        {
            var tokens = Parse("1 = monday or 2 = tuesday");

            Assert.AreEqual(StatementType.Numeric, tokens[0].TokenType);
            Assert.AreEqual(StatementType.Word, tokens[1].TokenType);
            Assert.AreEqual(StatementType.Equality, tokens[2].TokenType);
            Assert.AreEqual(StatementType.Numeric, tokens[3].TokenType);
            Assert.AreEqual(StatementType.Word, tokens[4].TokenType);
            Assert.AreEqual(StatementType.Equality, tokens[5].TokenType);
            Assert.AreEqual(StatementType.Or, tokens[6].TokenType);
        }

        private static Token[] Parse(string query)
        {
            var parser = new RdlWhereParser();
            var dec = new LexerComplexTokensDecorator(query);
            return parser.Parse(dec);
        }
    }
}
