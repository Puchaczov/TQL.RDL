using System.Collections.Generic;
using System.Collections;
using TQL.Core.Syntax;
using TQL.RDL.Parser.Tokens;
using static TQL.RDL.Parser.Lexer;

namespace TQL.RDL.Parser
{
    public class LexerComplexTokensDecorator : ILexer<Token>, IEnumerable<Token>
    {
        private Lexer lexer;
        private StatementType[] disableEnumerationForTokens;
        private bool skipWhiteSpaces;

        public LexerComplexTokensDecorator(Lexer lexer)
        {
            this.lexer = lexer;
            disableEnumerationForTokens = new StatementType[0];
            skipWhiteSpaces = true;
        }

        public LexerComplexTokensDecorator(string input, DefinitionSet ds = DefinitionSet.Query)
        {
            lexer = new Lexer(input, ds);
            disableEnumerationForTokens = new StatementType[0];
            skipWhiteSpaces = true;
        }

        public int Position => lexer.Position;

        public string Query => lexer.Query;

        public void ChangePosition(int newPosition)
        {
            lexer.ChangePosition(newPosition);
        }

        public Token CurrentToken() => lexer.CurrentToken();

        public IEnumerator<Token> GetEnumerator() => new LexerEnumerator(this, disableEnumerationForTokens);

        public void DisableEnumerationWhen(params StatementType[] tokens)
        {
            disableEnumerationForTokens = tokens;
        }

        public void EnableEnumerationForAll()
        {
            disableEnumerationForTokens = new StatementType[0];
        }

        public Token LastToken() => lexer.LastToken();

        public Token NextToken()
        {
            var token = lexer.NextToken();
            while(skipWhiteSpaces && token.TokenType == StatementType.WhiteSpace)
            {
                token = lexer.NextToken();
            }
            return token;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
