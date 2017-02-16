using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDL.Parser;
using RDL.Parser.Tokens;
using TQL.Core.Syntax;

namespace TQL.RDL.Parser.Tests
{
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void CheckProducedTokens_ShouldPass()
        {
            var query = "repeat every seconds where day in (mon-fri) and month <> january start at @now";
            var lexer = new Lexer(query, true);
            var tokens = Tokenize(lexer);

            CheckTokenized_ShouldReturnOrderedToken(query, tokens,
                StatementType.Repeat,
                StatementType.Every,
                StatementType.Word,
                StatementType.Where,
                StatementType.Word,
                StatementType.In,
                StatementType.LeftParenthesis,
                StatementType.Word,
                StatementType.Hyphen,
                StatementType.Word,
                StatementType.RightParenthesis,
                StatementType.And,
                StatementType.Word,
                StatementType.Diff,
                StatementType.Word,
                StatementType.StartAt,
                StatementType.Var,
                StatementType.EndOfFile);
        }

        [TestMethod]
        public void CheckProducedTokents_CaseWhenQuery_ShouldPass()
        {
            var query = "repeat every day where @a > (case when (1 <> 2) then (5) else (4) esac)";
            var lexer = new Lexer(query, true);
            var tokens = Tokenize(lexer);

            CheckTokenized_ShouldReturnOrderedToken(query, tokens,
                StatementType.Repeat,
                StatementType.Every,
                StatementType.Word,
                StatementType.Where,
                StatementType.Var,
                StatementType.Greater,
                StatementType.LeftParenthesis,
                StatementType.Case,
                StatementType.When,
                StatementType.LeftParenthesis,
                StatementType.Numeric,
                StatementType.Diff,
                StatementType.Numeric,
                StatementType.RightParenthesis,
                StatementType.Then,
                StatementType.LeftParenthesis,
                StatementType.Numeric,
                StatementType.RightParenthesis,
                StatementType.Else,
                StatementType.LeftParenthesis,
                StatementType.Numeric,
                StatementType.RightParenthesis,
                StatementType.CaseEnd,
                StatementType.RightParenthesis,
                StatementType.EndOfFile);
        }

        [TestMethod]
        public void CheckProducedTokens_WithMultiWordKeyword_ShouldPass()
        {
            var query = "repeat every day start at @var stop at 123213";
            var lexer = new Lexer(query, true);
            var tokens = Tokenize(lexer);

            CheckTokenized_ShouldReturnOrderedToken(query, tokens,
                StatementType.Repeat,
                StatementType.Every,
                StatementType.Word,
                StatementType.StartAt,
                StatementType.Var,
                StatementType.StopAt,
                StatementType.Numeric,
                StatementType.EndOfFile);
        }

        [TestMethod]
        public void CheckProducedTokens_WithFunctionCall_ShouldPass()
        {
            var query = "repeat every day where myfun(1,2)";
            var lexer = new Lexer(query, true);
            var tokens = Tokenize(lexer);

            CheckTokenized_ShouldReturnOrderedToken(query, tokens,
                StatementType.Repeat,
                StatementType.Every,
                StatementType.Word,
                StatementType.Where,
                StatementType.Function,
                StatementType.LeftParenthesis,
                StatementType.Numeric,
                StatementType.Comma,
                StatementType.Numeric,
                StatementType.RightParenthesis,
                StatementType.EndOfFile);
        }

        [TestMethod]
        public void CheckProducedTokens_WithInOperator_ShouldPass()
        {
            var query = "RePeat eVery dAy whEre @year IN (@year, 2013, 'WORD')";
            var lexer = new Lexer(query, true);
            var tokens = Tokenize(lexer);

            CheckTokenized_ShouldReturnOrderedToken(query, tokens,
                StatementType.Repeat,
                StatementType.Every,
                StatementType.Word,
                StatementType.Where,
                StatementType.Var,
                StatementType.In,
                StatementType.LeftParenthesis,
                StatementType.Var,
                StatementType.Comma,
                StatementType.Numeric,
                StatementType.Comma,
                StatementType.Word,
                StatementType.RightParenthesis,
                StatementType.EndOfFile);
        }

        private static Token[] Tokenize(ILexer<Token> lexer)
        {
            var lst = new List<Token>();

            Token current = null;
            while ((current = lexer.Next()).TokenType != StatementType.EndOfFile)
                lst.Add(current);
            lst.Add(current);
            return lst.ToArray();
        }

        private static void CheckTokenized_ShouldReturnOrderedToken(string query, Token[] tokens,
            params StatementType[] types)
        {
            for (var i = 0; i < tokens.Length; ++i)
            {
                var span = tokens[i].Span;
                var substr = query.Substring(span.Start, span.Length);
                Assert.AreEqual(tokens[i].ToString().ToLowerInvariant(), substr.ToLowerInvariant());
                Assert.AreEqual(types[i], tokens[i].TokenType);
            }
        }
    }
}