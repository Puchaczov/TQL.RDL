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
            var lexer = new LexerComplexTokensDecorator("repeat every seconds where day in (mon-fri) and month <> january `start at` @now");
            var tokens = Tokenize(lexer);

            CheckTokenized_ShouldReturnOrderedToken(tokens,
                SyntaxType.Repeat,
                SyntaxType.Every,
                SyntaxType.Word,
                SyntaxType.Where,
                SyntaxType.Word,
                SyntaxType.In,
                SyntaxType.LeftParenthesis,
                SyntaxType.Word,
                SyntaxType.Minus,
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

        private static void CheckTokenized_ShouldReturnOrderedToken(Token[] tokens, params SyntaxType[] types)
        {
            for(int i = 0; i < tokens.Length; ++i)
            {
                Assert.IsTrue(tokens[i].TokenType == types[i]);
            }
        }
    }
}
