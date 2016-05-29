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
            operators.Add(new Token("or", SyntaxType.Or, new TextSpan(0, 0)), new PrecedenceAssociativity(0, Associativity.Left));
            operators.Add(new Token("and", SyntaxType.And, new TextSpan(0, 0)), new PrecedenceAssociativity(5, Associativity.Left));
            operators.Add(new Token("in", SyntaxType.In, new TextSpan(0, 0)), new PrecedenceAssociativity(10, Associativity.Left));
            operators.Add(new Token("=", SyntaxType.Equality, new TextSpan(0, 0)), new PrecedenceAssociativity(10, Associativity.Left));
            operators.Add(new Token("<>", SyntaxType.Diff, new TextSpan(0, 0)), new PrecedenceAssociativity(10, Associativity.Left));
            operators.Add(new Token(">", SyntaxType.Greater, new TextSpan(0, 0)), new PrecedenceAssociativity(10, Associativity.Left));
            operators.Add(new Token(">=", SyntaxType.GreaterEqual, new TextSpan(0, 0)), new PrecedenceAssociativity(10, Associativity.Left));
            operators.Add(new Token("<", SyntaxType.Less, new TextSpan(0, 0)), new PrecedenceAssociativity(10, Associativity.Left));
            operators.Add(new Token("<=", SyntaxType.LessEqual, new TextSpan(0, 0)), new PrecedenceAssociativity(10, Associativity.Left));
            operators.Add(new Token("not", SyntaxType.Not, new TextSpan(0, 0)), new PrecedenceAssociativity(15, Associativity.Right));
        }

        public override Token[] Parse(LexerComplexTokensDecorator expression) => InfixToPostfix(expression);
        protected override bool IsLeftParenthesis(Token token) => token.TokenType == SyntaxType.LeftParenthesis;
        protected override bool IsRightParenthesis(Token token) => token.TokenType == SyntaxType.RightParenthesis;
        protected override bool IsSkippable(Token token) => token.TokenType == SyntaxType.WhiteSpace;
        protected override bool IsComma(Token token) => token.TokenType == SyntaxType.Comma;
        protected override bool IsWord(Token token) => token.TokenType == SyntaxType.Word;
        protected override bool IsOperator(Token token) => this.operators.ContainsKey(token);
        protected override bool IsValue(Token token) => Regex.IsMatch("[a-Z1-9]+", token.Value) && !token.Value.Contains("@");

        protected override Token GenerateVarArgToken(int argsCount) => new VarArgToken(argsCount);

        protected override Token GenerateFunctionToken(Token oldToken) => new FunctionToken(oldToken.Value, oldToken.Span);

    }
}
