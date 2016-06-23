using System.Collections.Generic;
using System.Collections;
using TQL.Core.Syntax;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser
{
    public class LexerComplexTokensDecorator : ILexer<Token>, IEnumerable<Token>
    {
        private Lexer lexer;
        private SyntaxType[] disableEnumerationForTokens;

        public LexerComplexTokensDecorator(Lexer lexer)
        {
            this.lexer = lexer;
            disableEnumerationForTokens = new SyntaxType[0];
        }

        public LexerComplexTokensDecorator(string input)
        {
            lexer = new Lexer(input);
            disableEnumerationForTokens = new SyntaxType[0];
        }

        public int Position => lexer.Position;

        public Token CurrentToken() => lexer.CurrentToken();

        public IEnumerator<Token> GetEnumerator() => new LexerEnumerator(this, disableEnumerationForTokens);

        public void DisableEnumerationWhen(params SyntaxType[] tokens)
        {
            disableEnumerationForTokens = tokens;
        }

        public void EnableEnumerationForAll()
        {
            disableEnumerationForTokens = new SyntaxType[0];
        }

        public Token LastToken() => lexer.LastToken();

        public Token NextToken()
        {
            var token = lexer.NextToken();
            switch(token.TokenType)
            {
                case SyntaxType.Word:
                    return DetectKeywords(token);
                default:
                    return token;
            }
        }

        private Token DetectKeywords(Token token)
        {
            var value = token.Value.ToLowerInvariant();
            switch(value)
            {
                case "and":
                    return new AndToken(token.Span);
                case "or":
                    return new OrToken(token.Span);
                case "in":
                    return new InToken(token.Span);
                case "where":
                    return new WhereToken(token.Span);
                case "every":
                    return new EveryToken(token.Span);
                case "repeat":
                    return new RepeatToken(token.Span);
                case "start at":
                    return new StartAtToken(token.Span);
                case "stop at":
                    return new StopAtToken(token.Span);
                case "day":
                case "days":
                case "month":
                case "months":
                case "year":
                case "years":
                case "hour":
                case "hours":
                case "minute":
                case "minutes":
                case "second":
                case "seconds":
                    return new WordToken(value, token.Span);
                case "then":
                    return new ThenToken(token.Span);
                case "else":
                    return new ElseToken(token.Span);
                case "esac":
                    return new CaseEndToken(token.Span);
                case "when":
                    return new WhenToken(token.Span);
                case "case":
                    return new CaseToken(token.Span);
                default:
                    return token;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
