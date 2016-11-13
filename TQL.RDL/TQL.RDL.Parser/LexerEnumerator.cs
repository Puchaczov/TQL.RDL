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

        public LexerEnumerator(ILexer<Token> lexer, params StatementType[] abortOnToken)
        {
            this.lexer = lexer;
            this.abortOnToken = abortOnToken;
        }

        public object Current => current;

        Token IEnumerator<Token>.Current => current;

        public bool MoveNext()
        {
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