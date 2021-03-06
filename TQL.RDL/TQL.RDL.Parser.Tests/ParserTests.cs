﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TQL.RDL.Parser.Nodes;
using TQL.RDL.Parser.Tests.Resolvers;

namespace TQL.RDL.Parser.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void RDLParser_CheckIncompletedQuery_ShouldPass()
        {
            Parse("repeat");
        }

        [TestMethod]
        public void RDLParser_ComposeRepeatAt_ShouldPass()
        {
            var node = Parse("repeat every 5 seconds");

            Assert.AreEqual(1, node.Descendants.Length);
            Assert.AreEqual(typeof(NumericConsequentRepeatEveryNode), node.Descendants[0].GetType());

            var repeatNode = node.Descendants[0] as NumericConsequentRepeatEveryNode;

            Assert.AreEqual(5, repeatNode.Value);
        }

        [TestMethod]
        public void RDLParser_ComposeWhere_OperatorPrecendenceChangedByParenthesis()
        {
            var node = Parse("repeat every seconds where (1 + 2) * 3 = 0");

            Assert.AreEqual(2, node.Descendants.Length);
            Assert.AreEqual(typeof(RepeatEveryNode), node.Descendants[0].GetType());
            Assert.AreEqual(typeof(WhereConditionsNode), node.Descendants[1].GetType());

            Assert.AreEqual(typeof(EqualityNode), node.Descendants[1].Descendants[0].GetType());
            Assert.AreEqual(typeof(StarNode), node.Descendants[1].Descendants[0].Descendants[0].GetType());
            Assert.AreEqual(typeof(AddNode), node.Descendants[1].Descendants[0].Descendants[0].Descendants[0].GetType());
            Assert.AreEqual(typeof(NumericNode), node.Descendants[1].Descendants[0].Descendants[1].GetType());
        }

        [TestMethod]
        public void RDLParser_ComposeWhereWithOperators_ShouldPass()
        {
            TestOperator_Simple<OrNode, NumericNode, NumericNode>("or", "1", "32");
            TestOperator_Simple<AndNode, NumericNode, NumericNode>("and", "1", "32");
            TestOperator_Simple<DiffNode, NumericNode, NumericNode>("<>", "1", "32");
            TestOperator_Simple<EqualityNode, NumericNode, NumericNode>("=", "1", "32");
            TestOperator_Simple<EqualityNode, AddNode, AddNode>("=", "7 + 5", "20 + 11");
            TestOperator_Simple<GreaterNode, NumericNode, NumericNode>(">", "1", "32");
            TestOperator_Simple<LessNode, NumericNode, NumericNode>("<", "1", "32");
            TestOperator_Simple<GreaterEqualNode, NumericNode, NumericNode>(">=", "1", "32");
            TestOperator_Simple<LessEqualNode, NumericNode, NumericNode>("<=", "1", "32");
            TestOperator_Simple<AddNode, NumericNode, NumericNode>("+", "1", "32");
            TestOperator_Simple<HyphenNode, NumericNode, NumericNode>("-", "1", "32");
            TestOperator_Simple<FSlashNode, NumericNode, NumericNode>("/", "1", "32");
            TestOperator_Simple<StarNode, NumericNode, NumericNode>("*", "1", "32");
        }

        [TestMethod]
        public void RDLParser_ComposeWhereWithMulitpleSimpleConditions_ShouldPass()
        {
            var node = Parse("repeat every seconds where @day = @monday and @month <> @january");

            Assert.AreEqual(2, node.Descendants.Length);
            Assert.AreEqual(typeof(RepeatEveryNode), node.Descendants[0].GetType());
            Assert.AreEqual(typeof(WhereConditionsNode), node.Descendants[1].GetType());

            Assert.IsTrue(node.Descendants[1].Descendants.OfType<AndNode>().Any());
            Assert.IsTrue(node.Descendants[1].Descendants[0].Descendants.OfType<EqualityNode>().Any());
            Assert.IsTrue(node.Descendants[1].Descendants[0].Descendants.OfType<DiffNode>().Any());

            var equalityNode = node.Descendants[1].Descendants[0].Descendants.OfType<EqualityNode>().First();
            Assert.IsTrue(equalityNode.Left.GetType() == typeof(VarNode));
            Assert.IsTrue(equalityNode.Right.GetType() == typeof(VarNode));
            Assert.IsTrue((equalityNode.Left as VarNode).Value == "day");
            Assert.IsTrue((equalityNode.Right as VarNode).Value == "monday");

            var diffNode = node.Descendants[1].Descendants[0].Descendants.OfType<DiffNode>().First();
            Assert.IsTrue(diffNode.Left.GetType() == typeof(VarNode));
            Assert.IsTrue(diffNode.Right.GetType() == typeof(VarNode));
            Assert.IsTrue((diffNode.Left as VarNode).Value == "month");
            Assert.IsTrue((diffNode.Right as VarNode).Value == "january");
        }

        [TestMethod]
        public void RDLParser_ComposeCaseWhen_ShouldPass()
        {
            var node = Parse(@"
                repeat every days where 
                    case 
                        when 1 > 2 and 2 > 1 then GetDay() > 1 
                        else GetDay() < 5 esac");

            Assert.AreEqual(typeof(CaseNode), node.Descendants[1].Descendants[0].GetType());
            Assert.AreEqual(typeof(WhenThenNode), node.Descendants[1].Descendants[0].Descendants[0].GetType());
            Assert.AreEqual(typeof(ElseNode), node.Descendants[1].Descendants[0].Descendants[1].GetType());
            Assert.AreEqual(typeof(WhenNode), node.Descendants[1].Descendants[0].Descendants[0].Descendants[0].GetType());
            Assert.AreEqual(typeof(ThenNode), node.Descendants[1].Descendants[0].Descendants[0].Descendants[1].GetType());
        }

        [TestMethod]
        public void RDLParser_ComposeWhereWithInCondition_ShouldPass()
        {
            var node = Parse("repeat every days where @day in ( 'mon' , 'tue' ) or @a <> @b");

            Assert.AreEqual(2, node.Descendants.Length);
            Assert.AreEqual(typeof(RepeatEveryNode), node.Descendants[0].GetType());
            Assert.AreEqual(typeof(WhereConditionsNode), node.Descendants[1].GetType());

            Assert.IsTrue(node.Descendants[1].Descendants.OfType<OrNode>().Any());
            Assert.IsTrue(node.Descendants[1].Descendants[0].Descendants.OfType<InNode>().Any());
            Assert.IsTrue(node.Descendants[1].Descendants[0].
                Descendants.OfType<DiffNode>().Any());

            var inNode = node.Descendants[1].Descendants[0].Descendants.OfType<InNode>().First();
            Assert.IsTrue(inNode.Left.GetType() == typeof(VarNode));
            Assert.IsTrue(inNode.Right.GetType() == typeof(ArgListNode));
            Assert.IsTrue((inNode.Left as VarNode).Value == "day");
            Assert.IsTrue((inNode.Right as ArgListNode).Descendants[0].Token.Value == "mon");
            Assert.IsTrue((inNode.Right as ArgListNode).Descendants[1].Token.Value == "tue");

            var diffNode = node.Descendants[1].Descendants[0].Descendants.OfType<DiffNode>().First();
            Assert.IsTrue(diffNode.Left.GetType() == typeof(VarNode));
            Assert.IsTrue(diffNode.Right.GetType() == typeof(VarNode));
            Assert.IsTrue((diffNode.Left as VarNode).Value == "a");
            Assert.IsTrue((diffNode.Right as VarNode).Value == "b");
        }

        [TestMethod]
        public void RDLParser_CheckBasicOperators_ShouldPass()
        {
            var node = Parse("repeat every days where @f1 > @f1 and @f2 >= @f2 and @f3 < @f3 and @f4 <= @f4");

            Assert.AreEqual(2, node.Descendants.Length);
            Assert.AreEqual(typeof(RepeatEveryNode), node.Descendants[0].GetType());
            Assert.AreEqual(typeof(WhereConditionsNode), node.Descendants[1].GetType());

            var where = node.Descendants[1];

            Assert.AreEqual("@f1 > @f1 and @f2 >= @f2 and @f3 < @f3 and @f4 <= @f4", where.ToString());
        }

        [TestMethod]
        public void RDLParser_CheckFunctionCall_ShouldPass()
        {
            var node = Parse("repeat every days where Abc(1,2,@day) > 0");

            Assert.AreEqual(2, node.Descendants.Length);
            Assert.AreEqual(typeof(RepeatEveryNode), node.Descendants[0].GetType());
            Assert.AreEqual(typeof(WhereConditionsNode), node.Descendants[1].GetType());

            var where = node.Descendants[1];

            Assert.AreEqual("Abc(1,2,@day) > 0", where.ToString());
        }

        [TestMethod]
        public void RDLParser_ComposeStartAt_ShouldPass()
        {
            var node = Parse("repeat every days start at @now");

            Assert.AreEqual(2, node.Descendants.Length);
            Assert.AreEqual(typeof(RepeatEveryNode), node.Descendants[0].GetType());
            Assert.AreEqual(typeof(StartAtNode), node.Descendants[1].GetType());
            Assert.IsTrue(node.Descendants[1].Descendants[0].GetType() == typeof(VarNode));
            Assert.IsTrue((node.Descendants[1].Descendants[0] as VarNode).Value == "now");
        }

        [TestMethod]
        public void RDLParser_ComposeStopAt_ShouldPass()
        {
            var node = Parse("repeat every days stop at '21.05.2016'");

            Assert.AreEqual(2, node.Descendants.Length);
            Assert.AreEqual(typeof(RepeatEveryNode), node.Descendants[0].GetType());
            Assert.AreEqual(typeof(StopAtNode), node.Descendants[1].GetType());

            Assert.IsTrue(node.Descendants[1].Descendants[0].GetType() == typeof(DateTimeNode));
            Assert.IsTrue((node.Descendants[1].Descendants[0] as DateTimeNode).DateTime ==
                          DateTimeOffset.Parse("2016.05.21 +00:00"));
        }

        [TestMethod]
        public void RDLParser_ComposeFunctionCall_ShouldPass()
        {
            var node = Parse("repeat every days where MyFunction4(@day, @month, 3 and 4) and @p <> @v");

            Assert.AreEqual(2, node.Descendants.Length);
            Assert.AreEqual(typeof(RepeatEveryNode), node.Descendants[0].GetType());
            Assert.AreEqual(typeof(WhereConditionsNode), node.Descendants[1].GetType());

            Assert.IsTrue(node.Descendants[1].Descendants.OfType<AndNode>().Any());
            Assert.IsTrue(node.Descendants[1].Descendants[0].Descendants.OfType<RawFunctionNode>().Any());
            Assert.IsTrue(node.Descendants[1].Descendants[0].Descendants.OfType<DiffNode>().Any());

            var functionNode = node.Descendants[1].Descendants[0].Descendants.OfType<RawFunctionNode>().First();

            Assert.AreEqual("MyFunction4", functionNode.Name);

            var argList = functionNode.Args;

            Assert.IsTrue(argList.Descendants[0].GetType() == typeof(VarNode));
            Assert.IsTrue(argList.Descendants[1].GetType() == typeof(VarNode));
            Assert.IsTrue(argList.Descendants[2].GetType() == typeof(AndNode));

            var andNode = argList.Descendants[2] as AndNode;

            Assert.IsTrue(andNode.Left.GetType() == typeof(NumericNode));
            Assert.IsTrue(andNode.Right.GetType() == typeof(NumericNode));
        }

        [TestMethod]
        public void RDLParser_ComplexQuery_ShouldPass()
        {
            var node = Parse("repeat every days where 1 <> 2 start at '11.01.2012' stop at '12.01.2013'");

            Assert.AreEqual(4, node.Descendants.Length);

            Assert.AreEqual(typeof(RepeatEveryNode), node.Descendants[0].GetType());
            Assert.AreEqual(typeof(WhereConditionsNode), node.Descendants[1].GetType());
            Assert.AreEqual(typeof(StartAtNode), node.Descendants[2].GetType());
            Assert.AreEqual(typeof(StopAtNode), node.Descendants[3].GetType());

            Assert.IsTrue(node.Descendants[1].Descendants.OfType<DiffNode>().Any());
            Assert.IsTrue(typeof(NumericNode) == node.Descendants[1].Descendants[0].Descendants[0].GetType());
            Assert.IsTrue(typeof(NumericNode) == node.Descendants[1].Descendants[0].Descendants[1].GetType());

            var startAt = node.Descendants[2] as StartAtNode;
            Assert.IsTrue(DateTimeOffset.Parse("2012.01.11 +00:00") == startAt.When);

            var stopAt = node.Descendants[3] as StopAtNode;
            Assert.IsTrue(DateTimeOffset.Parse("2013.01.12 +00:00") == stopAt.When);
        }

        [TestMethod]
        public void RDLParser_FunctionAsFunctionArgument_ShouldPass()
        {
            var node = Parse("repeat every days where A(B())");

            Assert.AreEqual(2, node.Descendants.Length);

            Assert.IsTrue(node.Descendants[1].Descendants.OfType<RawFunctionNode>().Any());
            Assert.IsTrue(typeof(RawFunctionNode) == node.Descendants[1].Descendants[0].GetType());
            Assert.IsTrue(typeof(RawFunctionNode) == node.Descendants[1].Descendants[0].Descendants[0].GetType());
        }

        [TestMethod]
        public void RDLParser_BetweenOperatorNumeric_ShouldPass()
        {
            var node = Parse("repeat every days where 1 between 2 and 3");

            Assert.AreEqual(2, node.Descendants.Length);

            Assert.IsInstanceOfType(node.Descendants[1].Descendants[0], typeof(BetweenNode));
            Assert.IsInstanceOfType(node.Descendants[1].Descendants[0].Descendants[0], typeof(NumericNode));
            Assert.IsInstanceOfType(node.Descendants[1].Descendants[0].Descendants[1], typeof(NumericNode));
            Assert.IsInstanceOfType(node.Descendants[1].Descendants[0].Descendants[2], typeof(NumericNode));
        }

        [TestMethod]
        public void RDLParser_BetweenOperatorComplex_ShouldPass()
        {
            var node = Parse("repeat every days where GetDay() between GetMonth() and GetYear()");

            Assert.IsInstanceOfType(node.Descendants[1].Descendants[0], typeof(BetweenNode));
            Assert.IsInstanceOfType(node.Descendants[1].Descendants[0].Descendants[0], typeof(RawFunctionNode));
            Assert.IsInstanceOfType(node.Descendants[1].Descendants[0].Descendants[1], typeof(RawFunctionNode));
            Assert.IsInstanceOfType(node.Descendants[1].Descendants[0].Descendants[2], typeof(RawFunctionNode));
        }

        private void TestOperator_Simple<TOperatorNode, TLeftOperand, TRightOperand>(string op, string left,
            string right)
        {
            var node = Parse(string.Format("repeat every seconds where {1} {0} {2}", op, left, right));

            Assert.AreEqual(2, node.Descendants.Length);
            Assert.AreEqual(typeof(RepeatEveryNode), node.Descendants[0].GetType());
            Assert.AreEqual(typeof(WhereConditionsNode), node.Descendants[1].GetType());

            Assert.IsTrue(node.Descendants[1].Descendants.OfType<TOperatorNode>().Any());
            Assert.IsTrue(node.Descendants[1].Descendants[0].Descendants.OfType<TLeftOperand>().Any());
            Assert.IsTrue(node.Descendants[1].Descendants[0].Descendants.OfType<TRightOperand>().Any());
        }

        private static RootScriptNode Parse(string query)
        {
            var lexer = new Lexer(query, true);
            var parser = new RdlParser(lexer, new string[1]
            {
                "dd.M.yyyy"
            }, new System.Globalization.CultureInfo("en-US"), new DummyDeclarationResolver(), new Dictionary<string, int>());
            return parser.ComposeRootComponents();
        }
    }
}