using System;
using System.Collections.Generic;
using System.Linq;
using TQL.Core.Syntax;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser
{
    internal class LexerEnumerator : IEnumerator<Token>
    {
        private ILexer<Token> lexer;
        private Token current;
        private StatementType[] abortOnToken;
        private bool firstValueEnumerated = false;

        public LexerEnumerator(ILexer<Token> lexer, params StatementType[] abortOnToken)
        {
            this.lexer = lexer;
            this.abortOnToken = abortOnToken;
            this.current = lexer.CurrentToken();
        }

        public object Current => current;

        Token IEnumerator<Token>.Current => current;

        public bool MoveNext()
        {
            if (!firstValueEnumerated && current.TokenType != StatementType.None)
            {
                if (abortOnToken.Contains(current.TokenType))
                    return false;
                firstValueEnumerated = true;
                return true;
            }

            firstValueEnumerated = true;
            current = lexer.NextToken();

            if (abortOnToken.Contains(current.TokenType))
                return false;

            switch(current.TokenType)
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