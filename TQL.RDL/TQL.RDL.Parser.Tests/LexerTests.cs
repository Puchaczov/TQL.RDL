using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TQL.Core.Syntax;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Tests
{
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void CheckProducedTokens_ShouldPass()
        {
            var query = "repeat every seconds where day in (mon-fri) and month <> january `start at` @now";
            var lexer = new LexerComplexTokensDecorator(query);
            var tokens = Tokenize(lexer);

            CheckTokenized_ShouldReturnOrderedToken(query, tokens,
                SyntaxType.Repeat,
                SyntaxType.Every,
                SyntaxType.Word,
                SyntaxType.Where,
                SyntaxType.Word,
                SyntaxType.In,
                SyntaxType.LeftParenthesis,
                SyntaxType.Word,
                SyntaxType.Hyphen,
                SyntaxType.Word,
                SyntaxType.RightParenthesis,
                SyntaxType.And,
                SyntaxType.Word,
                SyntaxType.Diff,
                SyntaxType.Word,
                SyntaxType.StartAt,
                SyntaxType.Var,
                SyntaxType.EndOfFile);
        }

        [TestMethod]
        public void CheckProducedTokens_WithMultiWordKeyword_ShouldPass()
        {
            var query = "repeat every day start at @var stop at '123213'";
            var lexer = new LexerComplexTokensDecorator(query);
            var tokens = Tokenize(lexer);

            CheckTokenized_ShouldReturnOrderedToken(query, tokens,
                SyntaxType.Repeat,
                SyntaxType.Every,
                SyntaxType.Word,
                SyntaxType.StartAt,
                SyntaxType.Var,
                SyntaxType.StopAt,
                SyntaxType.Word,
                SyntaxType.EndOfFile);
        }

        [TestMethod]
        public void CheckProducedTokens_WithFunctionCall_ShouldPass()
        {
            var query = "repeat every day where myfun(1,2)";
            var lexer = new LexerComplexTokensDecorator(query);
            var tokens = Tokenize(lexer);

            CheckTokenized_ShouldReturnOrderedToken(query, tokens,
                SyntaxType.Repeat,
                SyntaxType.Every,
                SyntaxType.Word,
                SyntaxType.Where,
                SyntaxType.Word,
                SyntaxType.LeftParenthesis,
                SyntaxType.Numeric,
                SyntaxType.Comma,
                SyntaxType.Numeric,
                SyntaxType.RightParenthesis,
                SyntaxType.EndOfFile);
        }

        [TestMethod]
        public void CheckProducedTokens_WithInOperator_ShouldPass()
        {
            var query = "RePeat eVery dAy whEre @year IN (@year, 2013, 'WORD')";
            var lexer = new LexerComplexTokensDecorator(query);
            var tokens = Tokenize(lexer);

            CheckTokenized_ShouldReturnOrderedToken(query, tokens,
                SyntaxType.Repeat,
                SyntaxType.Every,
                SyntaxType.Word,
                SyntaxType.Where,
                SyntaxType.Var,
                SyntaxType.In,
                SyntaxType.LeftParenthesis,
                SyntaxType.Var,
                SyntaxType.Comma,
                SyntaxType.Numeric,
                SyntaxType.Comma,
                SyntaxType.Word,
                SyntaxType.RightParenthesis,
                SyntaxType.EndOfFile);
        }

        private static Token[] Tokenize(ILexer<Token> lexer)
        {
            List<Token> lst = new List<Token>();

            Token current = null;
            while ((current = lexer.NextToken()).TokenType != SyntaxType.EndOfFile)
            {
                lst.Add(current);
            }
            lst.Add(current);
            return lst.ToArray();
        }

        private static void CheckTokenized_ShouldReturnOrderedToken(string query, Token[] tokens, params SyntaxType[] types)
        {
            for(int i = 0; i < tokens.Length; ++i)
            {
                var span = tokens[i].Span;
                var substr = query.Substring(span.Start, span.Length);
                Assert.AreEqual(tokens[i].ToString().ToLowerInvariant(), substr.ToLowerInvariant());
                Assert.IsTrue(tokens[i].TokenType == types[i]);
            }
        }
    }
}
