using System;
using System.Collections.Generic;
using TQL.Core.Syntax;
using TQL.RDL.Parser.Nodes;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser
{
    public class RDLParser : ParserBase<Token, StatementType>
    {
        private LexerComplexTokensDecorator cLexer;

        public RDLParser(LexerComplexTokensDecorator lexer)
            : base(lexer)
        {
            lastToken = new NoneToken();
            currentToken = lexer.NextToken();
            cLexer = lexer;
        }

        public RootScriptNode ComposeRootComponents()
        {
            var rootComponents = new List<RdlSyntaxNode>();
            var i = 0;
            for(; currentToken.TokenType != StatementType.EndOfFile; ++i)
            {
                while(currentToken.TokenType == StatementType.WhiteSpace)
                {
                    Consume(StatementType.WhiteSpace);
                }
                rootComponents.Add(ComposeNextComponents());
            }
            return new RootScriptNode(rootComponents.ToArray());
        }

        private RdlSyntaxNode ComposeNextComponents()
        {
            switch(currentToken.TokenType)
            {
                case StatementType.Repeat:
                    Consume(StatementType.Repeat);
                    return ComposeRepeat();
                case StatementType.Where:
                    var where = ComposeWhere();
                    currentToken = lexer.CurrentToken();
                    return new WhereConditionsNode(where);
                case StatementType.StartAt:
                    Consume(StatementType.StartAt);
                    return ComposeStartAt();
                case StatementType.StopAt:
                    Consume(StatementType.StopAt);
                    return ComposeStopAt();
                default:
                    throw new NotSupportedException();
            }
        }

        private StartAtNode ComposeStartAt()
        {
            var startAtToken = lastToken;
            var token = currentToken;
            Consume(currentToken.TokenType);
            switch(token.TokenType)
            {
                case StatementType.Var:
                    return new StartAtNode(startAtToken, new VarNode(token as VarToken));
                case StatementType.Word:
                    return new StartAtNode(startAtToken, new DateTimeNode(token));
            }
            throw new NotSupportedException();
        }

        private RdlSyntaxNode ComposeStopAt()
        {
            var stopAtToken = lastToken;
            var token = currentToken;
            Consume(currentToken.TokenType);
            switch (token.TokenType)
            {
                case StatementType.Var:
                    throw new NotImplementedException();
                case StatementType.Word:
                    return new StopAtNode(stopAtToken, new DateTimeNode(token));
            }
            throw new NotSupportedException();
        }

        private RdlSyntaxNode ComposeWhere()
        {
            RDLWhereParser parser = new RDLWhereParser();
            cLexer.DisableEnumerationWhen(StatementType.StartAt, StatementType.StopAt);
            var tokens = parser.Parse(cLexer);
            cLexer.EnableEnumerationForAll();
            Stack<RdlSyntaxNode> nodes = new Stack<RdlSyntaxNode>();
            return ComposePostfix(nodes, tokens);
        }

        private RdlSyntaxNode ComposePostfix(Stack<RdlSyntaxNode> nodes, Token[] tokens)
        {
            for (int i = 0; i < tokens.Length; ++i)
            {
                switch (tokens[i].TokenType)
                {
                    case StatementType.Plus:
                        nodes.Push(new AddNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case StatementType.Hyphen:
                        nodes.Push(new HyphenNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case StatementType.Star:
                        nodes.Push(new StarNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case StatementType.Mod:
                        nodes.Push(new ModuloNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case StatementType.FSlash:
                        nodes.Push(new FSlashNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case StatementType.And:
                        nodes.Push(new AndNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case StatementType.Or:
                        nodes.Push(new OrNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case StatementType.Equality:
                        nodes.Push(new EqualityNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case StatementType.Diff:
                        nodes.Push(new DiffNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case StatementType.Greater:
                        nodes.Push(new GreaterNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case StatementType.GreaterEqual:
                        nodes.Push(new GreaterEqualNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case StatementType.Less:
                        nodes.Push(new LessNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case StatementType.LessEqual:
                        nodes.Push(new LessEqualNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case StatementType.In:
                    case StatementType.NotIn:
                        var args = nodes.Pop();
                        var partOfDate = nodes.Pop();
                        if (tokens[i].TokenType == StatementType.In)
                            nodes.Push(new InNode(partOfDate, args));
                        else
                            nodes.Push(new NotInNode(partOfDate, args));
                        break;
                    case StatementType.Numeric:
                        nodes.Push(new NumericNode(tokens[i]));
                        break;
                    case StatementType.Word:
                        nodes.Push(new WordNode(tokens[i]));
                        break;
                    case StatementType.Function:
                            nodes.Push(new FunctionNode(tokens[i] as FunctionToken, nodes.Pop() as ArgListNode));
                        break;
                    case StatementType.VarArg:
                        var varArg = tokens[i] as VarArgToken;
                        List<RdlSyntaxNode> arguments = new List<RdlSyntaxNode>();
                        for(int f = 0; f < varArg.Arguments; ++f)
                        {
                            arguments.Add(nodes.Pop());
                        }
                        arguments.Reverse();
                        nodes.Push(new ArgListNode(arguments));
                        break;
                    case StatementType.Var:
                        nodes.Push(new VarNode(tokens[i] as VarToken));
                        break;
                    case StatementType.Else:
                        var elseNode = new ElseNode(tokens[i], nodes.Pop());
                        var whenThenExpressions = new List<WhenThenNode>();
                        while (nodes.Peek() is WhenThenNode)
                        {
                            var whenNode = nodes.Pop() as WhenThenNode;
                            whenThenExpressions.Add(whenNode);
                        }
                        nodes.Push(new CaseNode(tokens[i], whenThenExpressions.ToArray(), elseNode));
                        break;
                    case StatementType.When:
                        nodes.Push(new WhenNode(tokens[i], nodes.Pop()));
                        break;
                    case StatementType.Then:
                        var thenNode = new ThenNode(tokens[i], nodes.Pop());
                        nodes.Push(new WhenThenNode(nodes.Pop(), thenNode));
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            return nodes.Pop();
        }

        private RdlSyntaxNode ComposeRepeat()
        {
            RepeatEveryNode node = null;
            var repeat = lastToken;
            switch(currentToken.TokenType)
            {
                case StatementType.Every:
                    var every = currentToken;
                    Consume(StatementType.Every);
                    if(currentToken.TokenType == StatementType.Numeric)
                    {
                        var numeric = currentToken;
                        Consume(StatementType.Numeric);
                        node = new NumericConsequentRepeatEveryNode(
                            new Token("repeat every", StatementType.Repeat, new Core.Tokens.TextSpan(repeat.Span.Start, currentToken.Span.End - repeat.Span.Start)), 
                            numeric as NumericToken, 
                            currentToken as WordToken);
                    }
                    else
                    {
                        node = new RepeatEveryNode(lastToken, currentToken);
                    }
                    Consume(currentToken.TokenType);
                    break;
                default:
                    node = new RepeatEveryNode(repeat, currentToken);
                    break;
            }
            return node;
        }

        private void EatWhiteSpaces()
        {
            while(currentToken.TokenType == StatementType.WhiteSpace)
            {
                Consume(currentToken.TokenType);
            }
        }
    }
}
