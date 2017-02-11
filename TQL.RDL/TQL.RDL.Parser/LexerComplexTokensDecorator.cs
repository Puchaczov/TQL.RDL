using System.Collections;
using System.Collections.Generic;
using RDL.Parser.Tokens;
using TQL.Core.Syntax;

namespace RDL.Parser
{
    public class LexerComplexTokensDecorator : ILexer<Token>, IEnumerable<Token>
    {
        private readonly Lexer _lexer;
        private readonly bool _skipWhiteSpaces;
        private StatementType[] _disableEnumerationForTokens;

        public LexerComplexTokensDecorator(Lexer lexer)
        {
            _lexer = lexer;
            _disableEnumerationForTokens = new StatementType[0];
            _skipWhiteSpaces = true;
        }

        public LexerComplexTokensDecorator(string input)
        {
            _lexer = new Lexer(input);
            _disableEnumerationForTokens = new StatementType[0];
            _skipWhiteSpaces = true;
        }

        public string Query => _lexer.Query;

        public IEnumerator<Token> GetEnumerator() => new LexerEnumerator(this, _disableEnumerationForTokens);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Position => _lexer.Position;

        public Token CurrentToken() => _lexer.CurrentToken();

        public Token LastToken() => _lexer.LastToken();

        public Token NextToken()
        {
            var token = _lexer.NextToken();
            while (_skipWhiteSpaces && token.TokenType == StatementType.WhiteSpace)
                token = _lexer.NextToken();
            return token;
        }

        public void ChangePosition(int newPosition)
        {
            _lexer.ChangePosition(newPosition);
        }

        public void DisableEnumerationWhen(params StatementType[] tokens)
        {
            _disableEnumerationForTokens = tokens;
        }

        public void EnableEnumerationForAll()
        {
            _disableEnumerationForTokens = new StatementType[0];
        }
    }
}