﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TQL.Core.Syntax;
using TQL.RDL.Parser.Nodes;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser
{
    public class RDLParser : ParserBase<Token, StatementType>
    {
        private LexerComplexTokensDecorator cLexer;
        private RdlMetadata metadatas;
        private TimeSpan zone;
        private string[] formats;
        private CultureInfo ci;

        public override Token CurrentToken
        {
            get;
            protected set;
        }

        public override Token LastToken
        {
            get;
            protected set;
        }

        protected override ILexer<Token> Lexer => cLexer;

        public RDLParser(LexerComplexTokensDecorator lexer, RdlMetadata metadatas, TimeSpan zone, string[] formats, CultureInfo ci)
        {
            cLexer = lexer;
            LastToken = new NoneToken();
            CurrentToken = lexer.NextToken();

            if (metadatas == null)
                throw new ArgumentNullException(nameof(RdlMetadata));

            this.metadatas = metadatas;

            this.zone = zone;
            this.formats = formats;
            this.ci = ci;
        }

        public RootScriptNode ComposeRootComponents()
        {
            var rootComponents = new List<RdlSyntaxNode>();
            var i = 0;
            for(; CurrentToken.TokenType != StatementType.EndOfFile; ++i)
            {
                while(CurrentToken.TokenType == StatementType.WhiteSpace)
                {
                    Consume(StatementType.WhiteSpace);
                }
                rootComponents.Add(ComposeNextComponents());
            }
            return new RootScriptNode(rootComponents.ToArray());
        }

        private RdlSyntaxNode ComposeNextComponents()
        {
            switch(CurrentToken.TokenType)
            {
                case StatementType.Repeat:
                    Consume(StatementType.Repeat);
                    return ComposeRepeat();
                case StatementType.Where:
                    var where = ComposeWhere();
                    CurrentToken = Lexer.CurrentToken();
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
            var startAtToken = LastToken;
            var token = CurrentToken;
            Consume(CurrentToken.TokenType);
            switch(token.TokenType)
            {
                case StatementType.Var:
                    return new StartAtNode(startAtToken, new VarNode(token as VarToken));
                case StatementType.Word:
                    return new StartAtNode(startAtToken, new DateTimeNode(token, zone, formats, ci));
            }
            throw new NotSupportedException();
        }

        private RdlSyntaxNode ComposeStopAt()
        {
            var stopAtToken = LastToken;
            var token = CurrentToken;
            Consume(CurrentToken.TokenType);
            switch (token.TokenType)
            {
                case StatementType.Var:
                    throw new NotImplementedException();
                case StatementType.Word:
                    return new StopAtNode(stopAtToken, new DateTimeNode(token, zone, formats, ci));
            }
            throw new NotSupportedException();
        }

        private RdlSyntaxNode ComposeWhere()
        {
            Consume(StatementType.Where);
            RDLWhereParser parser = new RDLWhereParser();
            cLexer.DisableEnumerationWhen(StatementType.StartAt, StatementType.StopAt);
            var tokens = parser.Parse(cLexer);
            cLexer.EnableEnumerationForAll();
            Stack<RdlSyntaxNode> nodes = new Stack<RdlSyntaxNode>();
            return ComposePostfix(nodes, tokens);
        }

        private RdlSyntaxNode ComposePostfix(Token[] tokens) => ComposePostfix(new Stack<RdlSyntaxNode>(), tokens);

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

                        //handling cases when there is only one argument (ie. "@a in (21)"). In such case, 
                        //ShuntingYard algorithm won't know if it point to casual expression or 'special' vararg expression (1,2,...).
                        if(!(args is ArgListNode))
                        {
                            args = new ArgListNode(new RdlSyntaxNode[] { args });
                        }

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
                        var functionToken = tokens[i] as FunctionToken;
                        var argsNode = nodes.Pop() as ArgListNode;
                        nodes.Push(new FunctionNode(functionToken, argsNode, () => metadatas.GetReturnType(functionToken.Value, argsNode.Descendants.Select(f => f.ReturnType).ToArray())));
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
                    case StatementType.CaseWhenEsac:
                        var oldLexer = cLexer;
                        cLexer = new LexerComplexTokensDecorator(new Lexer(cLexer.Query, Parser.Lexer.DefinitionSet.CaseWhen));
                        cLexer.ChangePosition(tokens[i].Span.Start);
                        nodes.Push(ComposeCaseWhenEsac());
                        cLexer = oldLexer;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            return nodes.Pop();
        }

        private RdlSyntaxNode ComposeCaseWhenEsac()
        {
            Consume(CurrentToken.TokenType);
            switch(CurrentToken.TokenType)
            {
                case StatementType.Case:
                    Consume(CurrentToken.TokenType);
                    return new CaseNode(LastToken, ConsumeWhenNodes(), ConsumeEndNode());
            }
            throw new NotSupportedException();
        }

        private WhenThenNode[] ConsumeWhenNodes()
        {
            List<WhenThenNode> nodes = new List<WhenThenNode>();
            RDLWhereParser parser = new RDLWhereParser();
            while (CurrentToken.TokenType == StatementType.When)
            {
                Consume(StatementType.When);
                var whenToken = LastToken;
                cLexer.DisableEnumerationWhen(StatementType.Then);
                var whenNode = parser.Parse(cLexer);
                CurrentToken = cLexer.CurrentToken();
                LastToken = cLexer.LastToken();
                var thenToken = CurrentToken;
                Consume(StatementType.Then);
                cLexer.DisableEnumerationWhen(StatementType.When, StatementType.Else);
                var thenNode = parser.Parse(cLexer);
                CurrentToken = cLexer.CurrentToken();
                LastToken = cLexer.LastToken();
                nodes.Add(new WhenThenNode(new WhenNode(whenToken, ComposePostfix(whenNode)), new ThenNode(thenToken, ComposePostfix(thenNode))));
            }

            return nodes.ToArray();
        }

        private ElseNode ConsumeEndNode()
        {
            Consume(StatementType.Else);
            cLexer.DisableEnumerationWhen(StatementType.CaseEnd);
            RDLWhereParser parser = new RDLWhereParser();
            var elseNode = parser.Parse(cLexer);
            CurrentToken = cLexer.CurrentToken();
            LastToken = cLexer.LastToken();
            Consume(StatementType.CaseEnd);
            return new ElseNode(LastToken, ComposePostfix(elseNode));
        }


        private RdlSyntaxNode ComposeRepeat()
        {
            RepeatEveryNode node = null;
            var repeat = LastToken;
            switch(CurrentToken.TokenType)
            {
                case StatementType.Every:
                    var every = CurrentToken;
                    Consume(StatementType.Every);
                    if(CurrentToken.TokenType == StatementType.Numeric)
                    {
                        var numeric = CurrentToken;
                        Consume(StatementType.Numeric);
                        node = new NumericConsequentRepeatEveryNode(
                            new Token("repeat every", StatementType.Repeat, new Core.Tokens.TextSpan(repeat.Span.Start, CurrentToken.Span.End - repeat.Span.Start)), 
                            numeric as NumericToken,
                            CurrentToken as WordToken);
                    }
                    else
                    {
                        node = new RepeatEveryNode(LastToken, CurrentToken);
                    }
                    Consume(CurrentToken.TokenType);
                    break;
                default:
                    node = new RepeatEveryNode(repeat, CurrentToken);
                    break;
            }
            return node;
        }

        private void EatWhiteSpaces()
        {
            while(CurrentToken.TokenType == StatementType.WhiteSpace)
            {
                Consume(CurrentToken.TokenType);
            }
        }
    }
}
