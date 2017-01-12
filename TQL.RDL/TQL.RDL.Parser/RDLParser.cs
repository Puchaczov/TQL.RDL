using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TQL.Core.Syntax;
using TQL.RDL.Parser.Nodes;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser
{
    public class RdlParser : ParserBase<Token, StatementType>
    {
        private LexerComplexTokensDecorator _cLexer;
        private readonly RdlMetadata _metadatas;
        private readonly TimeSpan _zone;
        private readonly string[] _formats;
        private readonly CultureInfo _ci;

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

        protected override ILexer<Token> Lexer => _cLexer;

        public RdlParser(LexerComplexTokensDecorator lexer, RdlMetadata metadatas, TimeSpan zone, string[] formats, CultureInfo ci)
        {
            _cLexer = lexer;
            LastToken = new NoneToken();
            CurrentToken = lexer.NextToken();

            if (metadatas == null)
                throw new ArgumentNullException(nameof(RdlMetadata));

            _metadatas = metadatas;

            _zone = zone;
            _formats = formats;
            _ci = ci;
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
                    return new StartAtNode(startAtToken, new DateTimeNode(token, _zone, _formats, _ci));
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
                    return new StopAtNode(stopAtToken, new DateTimeNode(token, _zone, _formats, _ci));
            }
            throw new NotSupportedException();
        }

        private RdlSyntaxNode ComposeWhere()
        {
            Consume(StatementType.Where);
            RdlWhereParser parser = new RdlWhereParser();
            _cLexer.DisableEnumerationWhen(StatementType.StartAt, StatementType.StopAt);
            var tokens = parser.Parse(_cLexer);
            _cLexer.EnableEnumerationForAll();
            Stack<RdlSyntaxNode> nodes = new Stack<RdlSyntaxNode>();
            return ComposePostfix(nodes, tokens);
        }

        private RdlSyntaxNode ComposePostfix(Token[] tokens) => ComposePostfix(new Stack<RdlSyntaxNode>(), tokens);

        private RdlSyntaxNode ComposePostfix(Stack<RdlSyntaxNode> nodes, Token[] tokens)
        {
            foreach (var t in tokens)
            {
                RdlSyntaxNode farg = null;
                RdlSyntaxNode sarg = null;
                switch (t.TokenType)
                {
                    case StatementType.Plus:
                        sarg = nodes.Pop();
                        farg = nodes.Pop();
                        nodes.Push(new AddNode(farg, sarg));
                        break;
                    case StatementType.Hyphen:
                        sarg = nodes.Pop();
                        farg = nodes.Pop();
                        nodes.Push(new HyphenNode(farg, sarg));
                        break;
                    case StatementType.Star:
                        sarg = nodes.Pop();
                        farg = nodes.Pop();
                        nodes.Push(new StarNode(farg, sarg));
                        break;
                    case StatementType.Mod:
                        sarg = nodes.Pop();
                        farg = nodes.Pop();
                        nodes.Push(new ModuloNode(farg, sarg));
                        break;
                    case StatementType.FSlash:
                        sarg = nodes.Pop();
                        farg = nodes.Pop();
                        nodes.Push(new FSlashNode(farg, sarg));
                        break;
                    case StatementType.And:
                        sarg = nodes.Pop();
                        farg = nodes.Pop();
                        nodes.Push(new AndNode(farg, sarg));
                        break;
                    case StatementType.Or:
                        sarg = nodes.Pop();
                        farg = nodes.Pop();
                        nodes.Push(new OrNode(farg, sarg));
                        break;
                    case StatementType.Equality:
                        sarg = nodes.Pop();
                        farg = nodes.Pop();
                        nodes.Push(new EqualityNode(farg, sarg));
                        break;
                    case StatementType.Diff:
                        sarg = nodes.Pop();
                        farg = nodes.Pop();
                        nodes.Push(new DiffNode(farg, sarg));
                        break;
                    case StatementType.Greater:
                        sarg = nodes.Pop();
                        farg = nodes.Pop();
                        nodes.Push(new GreaterNode(farg, sarg));
                        break;
                    case StatementType.GreaterEqual:
                        sarg = nodes.Pop();
                        farg = nodes.Pop();
                        nodes.Push(new GreaterEqualNode(farg, sarg));
                        break;
                    case StatementType.Less:
                        sarg = nodes.Pop();
                        farg = nodes.Pop();
                        nodes.Push(new LessNode(farg, sarg));
                        break;
                    case StatementType.LessEqual:
                        sarg = nodes.Pop();
                        farg = nodes.Pop();
                        nodes.Push(new LessEqualNode(farg, sarg));
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
                        if (t.TokenType == StatementType.In)
                            nodes.Push(new InNode(partOfDate, args));
                        else
                            nodes.Push(new NotInNode(partOfDate, args));
                        break;
                    case StatementType.Numeric:
                        nodes.Push(new NumericNode(t));
                        break;
                    case StatementType.Word:
                        nodes.Push(new WordNode(t));
                        break;
                    case StatementType.Function:
                        var functionToken = t as FunctionToken;
                        var argsNode = nodes.Pop() as ArgListNode;
                        nodes.Push(new FunctionNode(functionToken, argsNode, () => _metadatas.GetReturnType(functionToken.Value, argsNode.Descendants.Select(f => f.ReturnType).ToArray())));
                        break;
                    case StatementType.VarArg:
                        var varArg = t as VarArgToken;
                        var arguments = new List<RdlSyntaxNode>();
                        for(int f = 0; f < varArg.Arguments; ++f)
                        {
                            arguments.Add(nodes.Pop());
                        }
                        arguments.Reverse();
                        nodes.Push(new ArgListNode(arguments));
                        break;
                    case StatementType.Var:
                        nodes.Push(new VarNode(t as VarToken));
                        break;
                    case StatementType.Else:
                        var elseNode = new ElseNode(t, nodes.Pop());
                        var whenThenExpressions = new List<WhenThenNode>();
                        while (nodes.Peek() is WhenThenNode)
                        {
                            var whenNode = nodes.Pop() as WhenThenNode;
                            whenThenExpressions.Add(whenNode);
                        }
                        nodes.Push(new CaseNode(t, whenThenExpressions.ToArray(), elseNode));
                        break;
                    case StatementType.When:
                        nodes.Push(new WhenNode(t, nodes.Pop()));
                        break;
                    case StatementType.Then:
                        var thenNode = new ThenNode(t, nodes.Pop());
                        nodes.Push(new WhenThenNode(nodes.Pop(), thenNode));
                        break;
                    case StatementType.CaseWhenEsac:
                        var oldLexer = _cLexer;
                        _cLexer = new LexerComplexTokensDecorator(new Lexer(_cLexer.Query, Parser.Lexer.DefinitionSet.CaseWhen));
                        _cLexer.ChangePosition(t.Span.Start);
                        nodes.Push(ComposeCaseWhenEsac());
                        _cLexer = oldLexer;
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
            var nodes = new List<WhenThenNode>();
            var parser = new RdlWhereParser();
            while (CurrentToken.TokenType == StatementType.When)
            {
                Consume(StatementType.When);
                var whenToken = LastToken;
                _cLexer.DisableEnumerationWhen(StatementType.Then);
                var whenNode = parser.Parse(_cLexer);
                CurrentToken = _cLexer.CurrentToken();
                LastToken = _cLexer.LastToken();
                var thenToken = CurrentToken;
                Consume(StatementType.Then);
                _cLexer.DisableEnumerationWhen(StatementType.When, StatementType.Else);
                var thenNode = parser.Parse(_cLexer);
                CurrentToken = _cLexer.CurrentToken();
                LastToken = _cLexer.LastToken();
                nodes.Add(new WhenThenNode(new WhenNode(whenToken, ComposePostfix(whenNode)), new ThenNode(thenToken, ComposePostfix(thenNode))));
            }

            return nodes.ToArray();
        }

        private ElseNode ConsumeEndNode()
        {
            Consume(StatementType.Else);
            _cLexer.DisableEnumerationWhen(StatementType.CaseEnd);
            var parser = new RdlWhereParser();
            var elseNode = parser.Parse(_cLexer);
            CurrentToken = _cLexer.CurrentToken();
            LastToken = _cLexer.LastToken();
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
    }
}
