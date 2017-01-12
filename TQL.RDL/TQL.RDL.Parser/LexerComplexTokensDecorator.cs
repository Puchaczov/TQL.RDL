using System.Collections.Generic;
using System.Collections;
using TQL.Core.Syntax;
using TQL.RDL.Parser.Tokens;
using static TQL.RDL.Parser.Lexer;

namespace TQL.RDL.Parser
{
    public class LexerComplexTokensDecorator : ILexer<Token>, IEnumerable<Token>
    {
        private Lexer _lexer;
        private StatementType[] _disableEnumerationForTokens;
        private bool _skipWhiteSpaces;

        public LexerComplexTokensDecorator(Lexer lexer)
        {
            _lexer = lexer;
            _disableEnumerationForTokens = new StatementType[0];
            _skipWhiteSpaces = true;
        }

        public LexerComplexTokensDecorator(string input, DefinitionSet ds = DefinitionSet.Query)
        {
            _lexer = new Lexer(input, ds);
            _disableEnumerationForTokens = new StatementType[0];
            _skipWhiteSpaces = true;
        }

        public int Position => _lexer.Position;

        public string Query => _lexer.Query;

        public void ChangePosition(int newPosition)
        {
            _lexer.ChangePosition(newPosition);
        }

        public Token CurrentToken() => _lexer.CurrentToken();

        public IEnumerator<Token> GetEnumerator() => new LexerEnumerator(this, _disableEnumerationForTokens);

        public void DisableEnumerationWhen(params StatementType[] tokens)
        {
            _disableEnumerationForTokens = tokens;
        }

        public void EnableEnumerationForAll()
        {
            _disableEnumerationForTokens = new StatementType[0];
        }

        public Token LastToken() => _lexer.LastToken();

        public Token NextToken()
        {
            var token = _lexer.NextToken();
            while(_skipWhiteSpaces && token.TokenType == StatementType.WhiteSpace)
            {
                token = _lexer.NextToken();
            }
            return token;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
