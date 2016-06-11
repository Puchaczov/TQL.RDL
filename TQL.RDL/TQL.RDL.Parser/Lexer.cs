using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TQL.Core.Syntax;
using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser
{
    public class Lexer : LexerBase<Token>
    {
        private Dictionary<string, string> multiWordKeywords;
        private Dictionary<string, SyntaxType> multiWordKeywordsSyntaxMapper;

        public Lexer(string input) : 
            base(input, new NoneToken())
        {
            multiWordKeywords = new Dictionary<string, string>();
            multiWordKeywords.Add("start", "at");
            multiWordKeywords.Add("stop", "at");
            multiWordKeywords.Add("not", "in");

            multiWordKeywordsSyntaxMapper = new Dictionary<string, SyntaxType>();
            multiWordKeywordsSyntaxMapper.Add("start at", SyntaxType.StartAt);
            multiWordKeywordsSyntaxMapper.Add("stop at", SyntaxType.StopAt);
            multiWordKeywordsSyntaxMapper.Add("not in", SyntaxType.NotIn);
        }

        public override Token NextToken()
        {
            bool useNextChar = true;
            while(useNextChar)
            {
                if (pos > input.Length - 1)
                {
                    AssignTokenOfType(() => new EndOfFileToken(new TextSpan(input.Length, 0)));
                    return currentToken;
                }

                var currentChar = input[pos];

                if(IsMultipleKeywordSign(currentChar))
                {
                    pos += 1;
                    var token = this.ConsumeTillEnd('`');
                    pos += 1;
                    return token;
                }

                if(IsTextStartEndSign(currentChar))
                {
                    pos += 1;
                    var token = this.ConsumeTillEnd('\'');
                    pos += 1;
                    return token;
                }

                if (IsLetter(currentChar))
                {
                    var token = ConsumeLetters();
                    if(token.Value.StartsWith("@") && token.Value.Length > 1)
                    {
                        return new VarToken(token.Span, token.Value.Split('@')[1]);
                    }
                    if(IsMulitkeywordCandidate(token))
                    {
                        var keyword = string.Empty;
                        var type = SyntaxType.None;
                        if(IsMultikeyword(token, ref keyword, ref type))
                        {
                            var pos = this.pos - keyword.Length;
                            switch (type)
                            {
                                case SyntaxType.StartAt:
                                    return AssignTokenOfType(() => new StartAtToken(new TextSpan(pos, keyword.Length)));
                                case SyntaxType.StopAt:
                                    return AssignTokenOfType(() => new StopAtToken(new TextSpan(pos, keyword.Length)));
                                case SyntaxType.NotIn:
                                    return AssignTokenOfType(() => new NotInToken(new TextSpan(pos, keyword.Length)));
                            }
                        }
                    }
                    return token;
                }

                if (IsDigit(currentChar))
                {
                    return this.ConsumeNumeric();
                }

                if (IsDiffOp(currentChar))
                {
                    return this.ConsumeDiffOperator();
                }

                if(IsGreaterEqualOp(currentChar))
                {
                    return this.ConsumeGreateEqualOperator();
                }

                if(IsLessEqualOp(currentChar))
                {
                    return this.ConsumeLessEqualOperator();
                }

                switch (currentChar)
                {
                    case ' ':
                        pos += 1;
                        continue;
                    case ',':
                        return new CommaToken(new TextSpan(pos++, 1));
                    case '(':
                        return new LeftParenthesisToken(new TextSpan(pos++, 1));
                    case ')':
                        return new RightParenthesisToken(new TextSpan(pos++, 1));
                    case '+':
                        return new PlusToken(new TextSpan(pos++, 1));
                    case '-':
                        return new HyphenToken(new TextSpan(pos++, 1));
                    case '%':
                        return new ModuloToken(new TextSpan(pos++, 1));
                    case '*':
                        return new StarToken(new TextSpan(pos++, 1));
                    case '/':
                        return new FSlashToken(new TextSpan(pos++, 1));
                    case '=':
                        return new EqualityToken(new TextSpan(pos++, 1));
                    case '>':
                        return new GreaterToken(new TextSpan(pos++, 1));
                    case '<':
                        return new LessToken(new TextSpan(pos++, 1));
                }

                useNextChar = false;
            }

            throw new NotSupportedException();
        }

        private bool IsMultikeyword(WordToken token, ref string keyword, ref SyntaxType type)
        {
            var pos = this.pos;
            var spaces = new StringBuilder();
            while(this.input.Length > pos && this.input[pos] == ' ')
            {
                spaces.Append(' ');
                pos += 1;
            }
            var builder = new StringBuilder();
            while(this.input.Length > pos && IsLetter(this.input[pos]))
            {
                builder.Append(this.input[pos]);
                pos += 1;
            }
            this.pos = pos;
            var keywordCandidate = builder.ToString();
            if(this.multiWordKeywords.ContainsKey(token.Value) && this.multiWordKeywords[token.Value] == keywordCandidate)
            {
                type = multiWordKeywordsSyntaxMapper[token.Value + ' ' + keywordCandidate];
                keyword = token.Value + spaces + keywordCandidate;
                return true;
            }
            type = SyntaxType.None;
            keyword = string.Empty;
            return false;
        }

        private bool IsMulitkeywordCandidate(WordToken token) => this.multiWordKeywords.ContainsKey(token.Value);

        private Token ConsumeGreateEqualOperator()
        {
            var oldPos = pos;
            pos += 2;
            return new GreaterEqualToken(new TextSpan(oldPos, 2));
        }

        private Token ConsumeLessEqualOperator()
        {
            var oldPos = pos;
            pos += 2;
            return new LessEqualToken(new TextSpan(oldPos, 2));
        }

        private bool IsGreaterEqualOp(char currentChar) => input[pos] == '>' && pos + 1 < input.Length && input[pos + 1] == '=';

        private bool IsLessEqualOp(char currentChar) => input[pos] == '<' && pos + 1 < input.Length && input[pos + 1] == '=';

        private bool IsTextStartEndSign(char currentChar) => currentChar == '\'';

        private Token ConsumeTillEnd(char pChar)
        {
            var startPos = pos;
            var cnt = input.Length;
            while (cnt > pos && input[pos] != pChar)
            {
                ++pos;
            }

            return AssignTokenOfType(() => new WordToken(input.Substring(startPos, pos - startPos), new TextSpan(startPos, pos - startPos))) as WordToken;
        }

        private DiffToken ConsumeDiffOperator()
        {
            var start = pos;
            pos += 2;
            return new DiffToken(new TextSpan(start, 2));
        }

        private bool IsDiffOp(char currentChar) => currentChar == '<' && input.Length > pos + 1 && input[pos + 1] == '>';

        private NumericToken ConsumeNumeric()
        {
            var startPos = pos;
            var cnt = input.Length;
            while (cnt > pos && IsDigit(input[pos]))
            {
                ++pos;
            }

            return AssignTokenOfType(() => new NumericToken(input.Substring(startPos, pos - startPos), new TextSpan(startPos, pos - startPos))) as NumericToken;
        }

        private WordToken ConsumeLetters()
        {
            var startPos = pos;
            var cnt = input.Length;
            while (cnt > pos && (IsLetter(input[pos]) || IsDigit(input[pos])))
            {
                ++pos;
            }

            return AssignTokenOfType(() => new WordToken(input.Substring(startPos, pos - startPos), new TextSpan(startPos, pos - startPos))) as WordToken;
        }

        private bool IsMultipleKeywordSign(char sign) => sign == '`';

        public override Token LastToken() => lastToken;

        public override Token CurrentToken() => currentToken;
    }
}
