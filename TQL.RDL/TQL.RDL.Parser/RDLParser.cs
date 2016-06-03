using System;
using System.Collections.Generic;
using TQL.Core.Syntax;
using TQL.RDL.Parser.Nodes;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser
{
    public class RDLParser : ParserBase<Token, SyntaxType>
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
            for(; currentToken.TokenType != SyntaxType.EndOfFile; ++i)
            {
                while(currentToken.TokenType == SyntaxType.WhiteSpace)
                {
                    Consume(SyntaxType.WhiteSpace);
                }
                rootComponents.Add(ComposeNextComponents());
            }
            return new RootScriptNode(rootComponents.ToArray());
        }

        private RdlSyntaxNode ComposeNextComponents()
        {
            switch(currentToken.TokenType)
            {
                case SyntaxType.Repeat:
                    Consume(SyntaxType.Repeat);
                    return ComposeRepeat();
                case SyntaxType.Where:
                    var where = ComposeWhere();
                    currentToken = lexer.CurrentToken();
                    return new WhereConditionsNode(where);
                case SyntaxType.StartAt:
                    Consume(SyntaxType.StartAt);
                    return ComposeStartAt();
                case SyntaxType.StopAt:
                    Consume(SyntaxType.StopAt);
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
                case SyntaxType.Var:
                    return new StartAtNode(startAtToken, new VarNode(token as VarToken));
                case SyntaxType.Word:
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
                case SyntaxType.Var:
                    return new StopAtNode(stopAtToken, new VarNode(token as VarToken));
                case SyntaxType.Word:
                    return new StopAtNode(stopAtToken, new DateTimeNode(token));
            }
            throw new NotSupportedException();
        }

        private RdlSyntaxNode ComposeWhere()
        {
            RDLWhereParser parser = new RDLWhereParser();
            cLexer.DisableEnumerationWhen(SyntaxType.StartAt, SyntaxType.StopAt);
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
                    case SyntaxType.And:
                        nodes.Push(new AndNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case SyntaxType.Or:
                        nodes.Push(new OrNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case SyntaxType.Equality:
                        nodes.Push(new EqualityNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case SyntaxType.Word:
                        nodes.Push(new WordNode(tokens[i]));
                        break;
                    case SyntaxType.Numeric:
                        nodes.Push(new NumericNode(tokens[i]));
                        break;
                    case SyntaxType.Diff:
                        nodes.Push(new DiffNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case SyntaxType.In:
                    case SyntaxType.NotIn:
                        var args = nodes.Pop();
                        var partOfDate = nodes.Pop();
                        if(tokens[i].TokenType == SyntaxType.In)
                            nodes.Push(new InNode(partOfDate, args));
                        else
                            nodes.Push(new NotInNode(partOfDate, args));
                        break;
                    case SyntaxType.Greater:
                        nodes.Push(new GreaterNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case SyntaxType.Less:
                        nodes.Push(new LessNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case SyntaxType.GreaterEqual:
                        nodes.Push(new GreaterEqualNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case SyntaxType.LessEqual:
                        nodes.Push(new LessEqualNode(nodes.Pop(), nodes.Pop()));
                        break;
                    case SyntaxType.Function:
                            nodes.Push(new FunctionNode(tokens[i] as FunctionToken, nodes.Pop() as ArgListNode));
                        break;
                    case SyntaxType.VarArg:
                        var varArg = tokens[i] as VarArgToken;
                        List<RdlSyntaxNode> arguments = new List<RdlSyntaxNode>();
                        for(int f = 0; f < varArg.Arguments; ++f)
                        {
                            arguments.Add(nodes.Pop());
                        }
                        arguments.Reverse();
                        nodes.Push(new ArgListNode(arguments));
                        break;
                    case SyntaxType.Var:
                        nodes.Push(new VarNode(tokens[i] as VarToken));
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
            switch(currentToken.TokenType)
            {
                case SyntaxType.Every:
                    Consume(SyntaxType.Every);
                    node = new RepeatEveryNode(lastToken, currentToken);
                    Consume(currentToken.TokenType);
                    break;
                case SyntaxType.Numeric:
                    var numeric = currentToken;
                    Consume(SyntaxType.Numeric);
                    if(numeric is NumericToken || currentToken is WordToken)
                    {
                        goto default;
                    }
                    node = new NumericConsequentRepeatEvery(lastToken, numeric as NumericToken, currentToken as WordToken);
                    Consume(SyntaxType.Word);
                    break;
                default:
                    throw new Exception("error");
            }
            return node;
        }

        private void EatWhiteSpaces()
        {
            while(currentToken.TokenType == SyntaxType.WhiteSpace)
            {
                Consume(currentToken.TokenType);
            }
        }
    }
}
