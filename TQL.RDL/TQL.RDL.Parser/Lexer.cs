using System.Collections.Generic;
using System.Text.RegularExpressions;
using TQL.Core.Syntax;
using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser
{
    public class Lexer : LexerBase<Token>
    {
        private static class TokenRegexDefinition
        {
            public static string Keyword = @"(?<=[\s]{{1,}}|^){0}(?=[\s]{{1,}}|$)";
            public static string Function = @"[a-zA-Z_-]{1,}[\d]*(?=[\(])";

            public static string KAnd = string.Format(Keyword, AndToken.TokenText);
            public static string KCase = string.Format(Keyword, CaseToken.TokenText);
            public static string KCaseEnd = string.Format(Keyword, CaseEndToken.TokenText);
            public static string KComma = CommaToken.TokenText;
            public static string KDiff = DiffToken.TokenText;
            public static string KElse = string.Format(Keyword, ElseToken.TokenText);
            public static string KEvery = string.Format(Keyword, EveryToken.TokenText);
            public static string KFSlashToken = string.Format(Keyword, FSlashToken.TokenText);
            public static string KGreater = string.Format(Keyword, GreaterToken.TokenText);
            public static string KGreaterEqual = string.Format(Keyword, GreaterEqualToken.TokenText);
            public static string KHyphen = string.Format(@"\{0}", HyphenToken.TokenText);
            public static string KIn = string.Format(Keyword, InToken.TokenText);
            public static string KIs = string.Format(Keyword, IsToken.TokenText);
            public static string KLeftParenthesis = string.Format(@"\{0}", LeftParenthesisToken.TokenText);
            public static string KLess = string.Format(Keyword, LessToken.TokenText);
            public static string KLessEqual = string.Format(Keyword, LessEqualToken.TokenText);
            public static string KModulo = string.Format(Keyword, ModuloToken.TokenText);
            public static string KNot = string.Format(Keyword, NotToken.TokenText);
            public static string KNotIn = string.Format(Keyword, NotInToken.TokenText);
            public static string KOr = string.Format(Keyword, OrToken.TokenText);
            public static string KPlus = string.Format(@"\{0}", PlusToken.TokenText);
            public static string KRepeat = string.Format(Keyword, RepeatToken.TokenText);
            public static string KRightParenthesis = string.Format(@"\{0}", RightParenthesisToken.TokenText);
            public static string KStar = string.Format(Keyword, string.Format(@"\{0}", StarToken.TokenText));
            public static string KStartAt = @"(?<=[\s]{1,}|^)start[\s]{1,}at(?=[\s]{1,}|$)";
            public static string KStopAt = @"(?<=[\s]{1,}|^)stop[\s]{1,}at(?=[\s]{1,}|$)";
            public static string KThen = string.Format(Keyword, ThenToken.TokenText);
            public static string KWhen = string.Format(Keyword, WhenToken.TokenText);
            public static string KWhere = string.Format(Keyword, WhereToken.TokenText);
            public static string KWhiteSpace = @"[\s]{1,}";
            public static string KVar = @"@(?<varname>(?<=@)((\w{1,})))(?<=\s{0,}|$)";
            public static string KWord = @"[\w*?_]{1,}";
            public static string KWordBracketed = @"'(.*?[^\\])'";
            public static string KEqual = string.Format(Keyword, EqualityToken.TokenText);
        }

        public Lexer(string input) : 
            base(input, new NoneToken(),
                new TokenDefinition(TokenRegexDefinition.KAnd),
                new TokenDefinition(TokenRegexDefinition.KCase),
                new TokenDefinition(TokenRegexDefinition.KCaseEnd),
                new TokenDefinition(TokenRegexDefinition.KComma),
                new TokenDefinition(TokenRegexDefinition.KDiff),
                new TokenDefinition(TokenRegexDefinition.KElse),
                new TokenDefinition(TokenRegexDefinition.KEvery),
                new TokenDefinition(TokenRegexDefinition.KFSlashToken),
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
                new TokenDefinition(TokenRegexDefinition.KThen),
                new TokenDefinition(TokenRegexDefinition.KWhen),
                new TokenDefinition(TokenRegexDefinition.KStar),
                new TokenDefinition(TokenRegexDefinition.KWhere),
                new TokenDefinition(TokenRegexDefinition.KWhiteSpace),
                new TokenDefinition(TokenRegexDefinition.KVar),
                new TokenDefinition(TokenRegexDefinition.KWordBracketed, RegexOptions.ECMAScript),
                new TokenDefinition(TokenRegexDefinition.KWord, RegexOptions.Singleline))
        { }

        protected override Token GetEndOfFileToken() => new EndOfFileToken(new TextSpan(input.Length, 0));

        protected override Token GetToken(TokenDefinition matchedDefinition, Match match)
        {
            string tokenText = match.Value;
            StatementType token = GetTokenCandidate(tokenText, matchedDefinition);

            switch(token)
            {
                case StatementType.And:
                    return new AndToken(new TextSpan(Position, tokenText.Length));
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

            var matched = Regex.Match(tokenText, TokenRegexDefinition.KWordBracketed);

            if (matched.Success)
                return new WordToken(matched.Groups[1].Value, new TextSpan(Position + 1, matched.Groups[1].Value.Length));
            return new WordToken(tokenText, new TextSpan(Position, tokenText.Length));
        }

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
            
            int number = 0;
            if (int.TryParse(tokenText, out number) && !tokenText.Contains(" "))
            {
                return StatementType.Numeric;
            }
            else if(Regex.IsMatch(tokenText, TokenRegexDefinition.Function))
            {
                return StatementType.Function;
            }
            else if(matchedDefinition.Regex.GroupNumberFromName("varname") != -1)
            {
                return StatementType.Var;
            }

            return StatementType.Word;
        }
    }
}
