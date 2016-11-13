using System.Linq;
using System.Text.RegularExpressions;
using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;
using UsefullAlgorithms.Parsing.ExpressionParsing;

namespace TQL.RDL.Parser
{
    public class RDLWhereParser : ShuntingYard<LexerComplexTokensDecorator, Token>
    {
        public RDLWhereParser()
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
            operators.Add(new WhenToken(new TextSpan()), new PrecedenceAssociativity(20, Associativity.Left));
            operators.Add(new ThenToken(new TextSpan()), new PrecedenceAssociativity(20, Associativity.Left));
            operators.Add(new ElseToken(new TextSpan()), new PrecedenceAssociativity(20, Associativity.Left));
        }

        public override Token[] Parse(LexerComplexTokensDecorator expression) => InfixToPostfix(expression);
        protected override bool IsLeftParenthesis(Token token) => token.TokenType == StatementType.LeftParenthesis || token.TokenType == StatementType.Case;
        protected override bool IsRightParenthesis(Token token) => token.TokenType == StatementType.RightParenthesis || token.TokenType == StatementType.CaseEnd;
        protected override bool IsSkippable(Token token) => token.TokenType == StatementType.WhiteSpace;
        protected override bool IsComma(Token token) => token.TokenType == StatementType.Comma;
        protected override bool IsWord(Token token) => token.TokenType == StatementType.Word;
        protected override bool IsOperator(Token token) => this.operators.ContainsKey(token);
        protected override bool IsValue(Token token) => Regex.IsMatch("[a-Z1-9]+", token.Value) && !token.Value.Contains("@");

        protected override Token GenerateVarArgToken(int argsCount) => new VarArgToken(argsCount);
        protected override Token GenerateFunctionToken(Token oldToken) => new FunctionToken(oldToken.Value, oldToken.Span);
    }
}
