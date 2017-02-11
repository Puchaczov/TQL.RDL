using System.Text.RegularExpressions;
using RDL.Parser.Tokens;
using TQL.Core.Syntax;
using TQL.Core.Tokens;

namespace RDL.Parser
{
    public class Lexer : LexerBase<Token>
    {
        private readonly bool _skipWhiteSpaces;

        /// <summary>
        /// Initialize instance.
        /// </summary>
        /// <param name="input">The query.</param>
        /// <param name="skipWhiteSpaces">Should skip whitespaces?</param>
        public Lexer(string input, bool skipWhiteSpaces) :
            base(input, new NoneToken(), DefinitionSets.General)
        {
            _skipWhiteSpaces = skipWhiteSpaces;
        }

        /// <summary>
        /// Gets passed query.
        /// </summary>
        public string Query => input;

        /// <summary>
        /// Changes position where lexer starts matching rules.
        /// </summary>
        /// <param name="newPosition"></param>
        public void ChangePosition(int newPosition)
        {
            Position = newPosition;
        }

        #region Overrides of LexerBase<Token>

        /// <summary>
        /// Gets the next token from tokens stream.
        /// </summary>
        /// <returns>The token.</returns>
        public override Token NextToken()
        {
            var token = base.NextToken();
            while (_skipWhiteSpaces && token.TokenType == StatementType.WhiteSpace)
                token = base.NextToken();
            return token;
        }

        /// <summary>
        /// Gets EndOfFile token.
        /// </summary>
        /// <returns>End of file token.</returns>
        protected override Token GetEndOfFileToken() => new EndOfFileToken(new TextSpan(input.Length, 0));

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="matchedDefinition">The definition of token type that fits requirements.</param>
        /// <param name="match">The match.</param>
        /// <returns>The token.</returns>
        protected override Token GetToken(TokenDefinition matchedDefinition, Match match)
        {
            var tokenText = match.Value;
            var token = GetTokenCandidate(tokenText, matchedDefinition);

            switch (token)
            {
                case StatementType.And:
                    return new AndToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Between:
                    return new BetweenToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Case:
                    return new CaseToken(new TextSpan(Position, tokenText.Length));
                case StatementType.CaseEnd:
                    return new CaseEndToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Comma:
                    return new CommaToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Diff:
                    return new DiffToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Else:
                    return new ElseToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Equality:
                    return new EqualityToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Every:
                    return new EveryToken(new TextSpan(Position, tokenText.Length));
                case StatementType.FSlash:
                    return new FSlashToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Function:
                    return new FunctionToken(tokenText, new TextSpan(Position, tokenText.Length));
                case StatementType.Greater:
                    return new GreaterToken(new TextSpan(Position, tokenText.Length));
                case StatementType.GreaterEqual:
                    return new GreaterEqualToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Hyphen:
                    return new HyphenToken(new TextSpan(Position, tokenText.Length));
                case StatementType.In:
                    return new InToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Is:
                    return new IsToken(new TextSpan(Position, tokenText.Length));
                case StatementType.LeftParenthesis:
                    return new LeftParenthesisToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Less:
                    return new LessToken(new TextSpan(Position, tokenText.Length));
                case StatementType.LessEqual:
                    return new LessEqualToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Mod:
                    return new ModuloToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Not:
                    return new NotToken(new TextSpan(Position, tokenText.Length));
                case StatementType.NotIn:
                    return new NotInToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Numeric:
                    return new NumericToken(tokenText, new TextSpan(Position, tokenText.Length));
                case StatementType.Or:
                    return new OrToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Plus:
                    return new PlusToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Repeat:
                    return new RepeatToken(new TextSpan(Position, tokenText.Length));
                case StatementType.RightParenthesis:
                    return new RightParenthesisToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Star:
                    return new StarToken(new TextSpan(Position, tokenText.Length));
                case StatementType.StartAt:
                    return new StartAtToken(new TextSpan(Position, tokenText.Length));
                case StatementType.StopAt:
                    return new StopAtToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Then:
                    return new ThenToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Var:
                    return new VarToken(tokenText.Substring(1), new TextSpan(Position, tokenText.Length));
                case StatementType.When:
                    return new WhenToken(new TextSpan(Position, tokenText.Length));
                case StatementType.Where:
                    return new WhereToken(new TextSpan(Position, tokenText.Length));
                case StatementType.WhiteSpace:
                    return new WhiteSpaceToken(new TextSpan(Position, tokenText.Length));
            }

            if (matchedDefinition.Regex.ToString() == TokenRegexDefinition.KWordBracketed)
                return new WordToken(match.Groups[1].Value, new TextSpan(Position + 1, match.Groups[1].Value.Length));
            return new WordToken(tokenText, new TextSpan(Position, tokenText.Length));
        }

        #endregion

        /// <summary>
        /// Resolve the statement type.
        /// </summary>
        /// <param name="tokenText">Text that match some definition</param>
        /// <param name="matchedDefinition">Definition that text matched.</param>
        /// <returns>Statement type.</returns>
        private StatementType GetTokenCandidate(string tokenText, TokenDefinition matchedDefinition)
        {
            switch (tokenText.ToLowerInvariant())
            {
                case AndToken.TokenText:
                    return StatementType.And;
                case CaseToken.TokenText:
                    return StatementType.Case;
                case CaseEndToken.TokenText:
                    return StatementType.CaseEnd;
                case BetweenToken.TokenText:
                    return StatementType.Between;
                case CommaToken.TokenText:
                    return StatementType.Comma;
                case DiffToken.TokenText:
                    return StatementType.Diff;
                case ElseToken.TokenText:
                    return StatementType.Else;
                case EqualityToken.TokenText:
                    return StatementType.Equality;
                case EveryToken.TokenText:
                    return StatementType.Every;
                case FSlashToken.TokenText:
                    return StatementType.FSlash;
                case GreaterToken.TokenText:
                    return StatementType.Greater;
                case GreaterEqualToken.TokenText:
                    return StatementType.GreaterEqual;
                case HyphenToken.TokenText:
                    return StatementType.Hyphen;
                case InToken.TokenText:
                    return StatementType.In;
                case IsToken.TokenText:
                    return StatementType.Is;
                case LeftParenthesisToken.TokenText:
                    return StatementType.LeftParenthesis;
                case LessToken.TokenText:
                    return StatementType.Less;
                case LessEqualToken.TokenText:
                    return StatementType.LessEqual;
                case ModuloToken.TokenText:
                    return StatementType.Mod;
                case NotToken.TokenText:
                    return StatementType.Not;
                case NotInToken.TokenText:
                    return StatementType.NotIn;
                case OrToken.TokenText:
                    return StatementType.Or;
                case PlusToken.TokenText:
                    return StatementType.Plus;
                case RepeatToken.TokenText:
                    return StatementType.Repeat;
                case RightParenthesisToken.TokenText:
                    return StatementType.RightParenthesis;
                case StarToken.TokenText:
                    return StatementType.Star;
                case StartAtToken.TokenText:
                    return StatementType.StartAt;
                case StopAtToken.TokenText:
                    return StatementType.StopAt;
                case ThenToken.TokenText:
                    return StatementType.Then;
                case WhenToken.TokenText:
                    return StatementType.When;
                case WhereToken.TokenText:
                    return StatementType.Where;
                case WhiteSpaceToken.TokenText:
                    return StatementType.WhiteSpace;
            }

            if (string.IsNullOrWhiteSpace(tokenText))
                return StatementType.WhiteSpace;

            if (matchedDefinition.Regex.ToString() == TokenRegexDefinition.Function)
                return StatementType.Function;
            var number = 0;
            if (int.TryParse(tokenText, out number) && !tokenText.Contains(" "))
                return StatementType.Numeric;
            if (matchedDefinition.Regex.GroupNumberFromName("varname") != -1)
                return StatementType.Var;

            return StatementType.Word;
        }

        /// <summary>
        /// The token regexes set.
        /// </summary>
        private static class TokenRegexDefinition
        {
            private const string Keyword = @"(?<=[\s]{{1,}}|^){0}(?=[\s]{{1,}}|$)";
            public const string Function = @"[a-zA-Z_-]{1,}[\d]*(?=[\(])";

            public static readonly string KAnd = string.Format(Keyword, AndToken.TokenText);
            public static readonly string KComma = CommaToken.TokenText;
            public static readonly string KDiff = DiffToken.TokenText;
            public static readonly string KElse = string.Format(Keyword, ElseToken.TokenText);
            public static readonly string KEvery = string.Format(Keyword, EveryToken.TokenText);
            public static readonly string KfSlashToken = string.Format(Keyword, FSlashToken.TokenText);
            public static readonly string KGreater = string.Format(Keyword, GreaterToken.TokenText);
            public static readonly string KGreaterEqual = string.Format(Keyword, GreaterEqualToken.TokenText);
            public static readonly string KHyphen = $@"\{HyphenToken.TokenText}";
            public static readonly string KIn = string.Format(Keyword, InToken.TokenText);
            public static readonly string KIs = string.Format(Keyword, IsToken.TokenText);
            public static readonly string KLeftParenthesis = $@"\{LeftParenthesisToken.TokenText}";
            public static readonly string KLess = string.Format(Keyword, LessToken.TokenText);
            public static readonly string KLessEqual = string.Format(Keyword, LessEqualToken.TokenText);
            public static readonly string KModulo = string.Format(Keyword, ModuloToken.TokenText);
            public static readonly string KNot = string.Format(Keyword, NotToken.TokenText);
            public static readonly string KNotIn = string.Format(Keyword, NotInToken.TokenText);
            public static readonly string KOr = string.Format(Keyword, OrToken.TokenText);
            public static readonly string KPlus = $@"\{PlusToken.TokenText}";
            public static readonly string KRepeat = string.Format(Keyword, RepeatToken.TokenText);
            public static readonly string KRightParenthesis = $@"\{RightParenthesisToken.TokenText}";
            public static readonly string KStar = string.Format(Keyword, $@"\{StarToken.TokenText}");
            public static readonly string KStartAt = @"(?<=[\s]{1,}|^)start[\s]{1,}at(?=[\s]{1,}|$)";
            public static readonly string KStopAt = @"(?<=[\s]{1,}|^)stop[\s]{1,}at(?=[\s]{1,}|$)";
            public static readonly string KThen = string.Format(Keyword, ThenToken.TokenText);
            public static readonly string KWhere = string.Format(Keyword, WhereToken.TokenText);
            public static readonly string KWhiteSpace = @"[\s]{1,}";
            public static readonly string KVar = @"@(?<varname>(?<=@)((\w{1,})))(?<=\s{0,}|$)";
            public static readonly string KWord = @"[\w*?_]{1,}";
            public static readonly string KWordBracketed = @"'(.*?[^\\])'";
            public static readonly string KEqual = string.Format(Keyword, EqualityToken.TokenText);
            public static readonly string KCaseWhenEsac = @"case.*esac";
            public static readonly string KCase = string.Format(Keyword, CaseToken.TokenText);
            public static readonly string KCaseEnd = string.Format(Keyword, CaseEndToken.TokenText);
            public static readonly string KWhen = string.Format(Keyword, WhenToken.TokenText);
            public static readonly string KBetweenAnd = @"(?<=between\s).+?(?=and)and\s+.*?(?=\s+and|\s+or|$)";
            public static readonly string KBetween = string.Format(Keyword, "between");
        }

        /// <summary>
        /// The token definitions set.
        /// </summary>
        private static class DefinitionSets
        {
            /// <summary>
            /// All supported by language keyword.
            /// </summary>
            public static TokenDefinition[] General => new[]
            {
                new TokenDefinition(TokenRegexDefinition.KAnd),
                new TokenDefinition(TokenRegexDefinition.KCase),
                new TokenDefinition(TokenRegexDefinition.KWhen),
                new TokenDefinition(TokenRegexDefinition.KThen),
                new TokenDefinition(TokenRegexDefinition.KCaseEnd),
                new TokenDefinition(TokenRegexDefinition.KBetween),
                new TokenDefinition(TokenRegexDefinition.KComma),
                new TokenDefinition(TokenRegexDefinition.KDiff),
                new TokenDefinition(TokenRegexDefinition.KElse),
                new TokenDefinition(TokenRegexDefinition.KEvery),
                new TokenDefinition(TokenRegexDefinition.KfSlashToken),
                new TokenDefinition(TokenRegexDefinition.KGreater),
                new TokenDefinition(TokenRegexDefinition.KGreaterEqual),
                new TokenDefinition(TokenRegexDefinition.KHyphen),
                new TokenDefinition(TokenRegexDefinition.KIn),
                new TokenDefinition(TokenRegexDefinition.KIs),
                new TokenDefinition(TokenRegexDefinition.KLeftParenthesis),
                new TokenDefinition(TokenRegexDefinition.KLess),
                new TokenDefinition(TokenRegexDefinition.KLessEqual),
                new TokenDefinition(TokenRegexDefinition.KEqual),
                new TokenDefinition(TokenRegexDefinition.KModulo),
                new TokenDefinition(TokenRegexDefinition.KNotIn),
                new TokenDefinition(TokenRegexDefinition.KNot),
                new TokenDefinition(TokenRegexDefinition.KOr),
                new TokenDefinition(TokenRegexDefinition.KPlus),
                new TokenDefinition(TokenRegexDefinition.KRepeat),
                new TokenDefinition(TokenRegexDefinition.KRightParenthesis),
                new TokenDefinition(TokenRegexDefinition.KStar),
                new TokenDefinition(TokenRegexDefinition.KStartAt),
                new TokenDefinition(TokenRegexDefinition.KStopAt),
                new TokenDefinition(TokenRegexDefinition.KStopAt),
                new TokenDefinition(TokenRegexDefinition.KStar),
                new TokenDefinition(TokenRegexDefinition.KWhere),
                new TokenDefinition(TokenRegexDefinition.KWhiteSpace),
                new TokenDefinition(TokenRegexDefinition.KVar),
                new TokenDefinition(TokenRegexDefinition.Function),
                new TokenDefinition(TokenRegexDefinition.KWordBracketed, RegexOptions.ECMAScript),
                new TokenDefinition(TokenRegexDefinition.KWord, RegexOptions.Singleline)
            };
        }
    }
}