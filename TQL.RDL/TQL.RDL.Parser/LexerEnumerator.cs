using System;
using System.Collections.Generic;
using System.Linq;
using RDL.Parser.Tokens;
using TQL.Core.Syntax;

namespace RDL.Parser
{
    internal class LexerEnumerator : IEnumerator<Token>
    {
        private readonly StatementType[] _abortOnToken;
        private Token _current;
        private bool _firstValueEnumerated;
        private readonly ILexer<Token> _lexer;

        public LexerEnumerator(ILexer<Token> lexer, params StatementType[] abortOnToken)
        {
            _lexer = lexer;
            _abortOnToken = abortOnToken;
            _current = lexer.CurrentToken();
        }

        public object Current => _current;

        Token IEnumerator<Token>.Current => _current;

        public bool MoveNext()
        {
            if (!_firstValueEnumerated && _current.TokenType != StatementType.None)
            {
                if (_abortOnToken.Contains(_current.TokenType))
                    return false;
                _firstValueEnumerated = true;
                return true;
            }

            _firstValueEnumerated = true;
            _current = _lexer.NextToken();

            if (_abortOnToken.Contains(_current.TokenType))
                return false;

            switch(_current.TokenType)
            {
                case StatementType.EndOfFile:
                    return false;
            }
            return true;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        { }
    }
}