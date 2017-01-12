using System.Linq;
using System.Text.RegularExpressions;
using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;
using UsefullAlgorithms.Parsing.ExpressionParsing;

namespace TQL.RDL.Parser
{
    public class RdlWhereParser : ShuntingYard<LexerComplexTokensDecorator, Token>
    {
        private static string[] _knownValues = new[] {
            "monday",
            "tuesday",
            "wednesday",
            "thursday",
            "friday",
            "saturday",
            "sunday"
        };

        public RdlWhereParser()
            : base()
        {
            operators.Add(new Token("or", StatementType.Or, new TextSpan(0, 0)), new PrecedenceAssociativity(0, Associativity.Left));
            operators.Add(new Token("and", StatementType.And, new TextSpan(0, 0)), new PrecedenceAssociativity(5, Associativity.Left));
            operators.Add(new Token("in", StatementType.In, new TextSpan(0, 0)), new PrecedenceAssociativity(10, Associativity.Left));
            operators.Add(new Token("not in", StatementType.NotIn, new TextSpan(0, 0)), new PrecedenceAssociativity(10, Associativity.Left));
            operators.Add(new Token("=", StatementType.Equality, new TextSpan(0, 0)), new PrecedenceAssociativity(10, Associativity.Left));
            operators.Add(new Token("<>", StatementType.Diff, new TextSpan(0, 0)), new PrecedenceAssociativity(10, Associativity.Left));
            operators.Add(new Token(">", StatementType.Greater, new TextSpan(0, 0)), new PrecedenceAssociativity(10, Associativity.Left));
            operators.Add(new Token(">=", StatementType.GreaterEqual, new TextSpan(0, 0)), new PrecedenceAssociativity(10, Associativity.Left));
            operators.Add(new Token("<", StatementType.Less, new TextSpan(0, 0)), new PrecedenceAssociativity(10, Associativity.Left));
            operators.Add(new Token("<=", StatementType.LessEqual, new TextSpan(0, 0)), new PrecedenceAssociativity(10, Associativity.Left));
            operators.Add(new Token("*", StatementType.Star, new TextSpan(0, 0)), new PrecedenceAssociativity(15, Associativity.Left));
            operators.Add(new Token("%", StatementType.Mod, new TextSpan(0, 0)), new PrecedenceAssociativity(15, Associativity.Left));
            operators.Add(new Token("/", StatementType.FSlash, new TextSpan(0, 0)), new PrecedenceAssociativity(15, Associativity.Left));
            operators.Add(new Token("-", StatementType.Hyphen, new TextSpan(0, 0)), new PrecedenceAssociativity(15, Associativity.Left));
            operators.Add(new Token("+", StatementType.Plus, new TextSpan(0, 0)), new PrecedenceAssociativity(14, Associativity.Left));
            operators.Add(new Token("not", StatementType.Not, new TextSpan(0, 0)), new PrecedenceAssociativity(20, Associativity.Right));
        }

        public override Token[] Parse(LexerComplexTokensDecorator expression) => InfixToPostfix(expression);

        protected override bool IsLeftParenthesis(Token token) => token.TokenType == StatementType.LeftParenthesis;
        protected override bool IsRightParenthesis(Token token) => token.TokenType == StatementType.RightParenthesis;
        protected override bool IsSkippable(Token token) => token.TokenType == StatementType.WhiteSpace;
        protected override bool IsComma(Token token) => token.TokenType == StatementType.Comma;
        protected override bool IsWord(Token token) => token.TokenType == StatementType.Word;
        protected override bool IsOperator(Token token) => operators.ContainsKey(token);
        protected override bool IsValue(Token token) => 
            (Regex.IsMatch("[a-Z1-9]+", token.Value) && !token.Value.Contains("@")) || 
            _knownValues.Contains(token.Value.ToLowerInvariant());

        protected override Token GenerateVarArgToken(int argsCount) => new VarArgToken(argsCount);
        protected override Token GenerateFunctionToken(Token oldToken) => new FunctionToken(oldToken.Value, oldToken.Span);
    }
}
