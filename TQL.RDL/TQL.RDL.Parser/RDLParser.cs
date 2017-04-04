using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using RDL.Parser.Exceptions;
using RDL.Parser.Helpers;
using RDL.Parser.Nodes;
using RDL.Parser.Tokens;
using TQL.Core.Exceptions;
using TQL.Core.Syntax;

namespace RDL.Parser
{
    public class RdlParser
    {
        private readonly CultureInfo _ci;
        private readonly Lexer _cLexer;
        private readonly string[] _formats;
        private readonly IMethodDeclarationResolver _resolver;

        public RdlParser(Lexer lexer, string[] formats, CultureInfo ci,
            IMethodDeclarationResolver resolver, IDictionary<int, int> functionCallOccurence)
        {
            _cLexer = lexer;
            
            _formats = formats;
            _ci = ci;
            _resolver = resolver;
            FunctionCallOccurence = functionCallOccurence;

            FunctionCallOccurence.Clear();
        }

        public IDictionary<int, int> FunctionCallOccurence { get; }

        private Token Current => _cLexer.Current();

        private Token Last => _cLexer.Last();

        private ILexer<Token> Lexer => _cLexer;

        public RootScriptNode ComposeRootComponents()
        {
            var rootComponents = new List<RdlSyntaxNode>();
            var i = 0;
            Consume(StatementType.None);
            for (; Current.TokenType != StatementType.EndOfFile; ++i)
            {
                while (Current.TokenType == StatementType.WhiteSpace)
                    Consume(StatementType.WhiteSpace);
                rootComponents.Add(ComposeSegmentComponents());
            }
            return new RootScriptNode(rootComponents.ToArray());
        }

        private RdlSyntaxNode ComposeSegmentComponents()
        {
            switch (Current.TokenType)
            {
                case StatementType.Repeat:
                    Consume(StatementType.Repeat);
                    return ComposeRepeat();
                case StatementType.Where:
                    Consume(StatementType.Where);
                    return new WhereConditionsNode(ComposeWhere());
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
            var startAtToken = Last;
            var token = Current;
            Consume(Current.TokenType);
            switch (token.TokenType)
            {
                case StatementType.Var:
                    return new StartAtNode(startAtToken, new VarNode(token as VarToken));
                case StatementType.Word:
                    return new StartAtNode(startAtToken, new DateTimeNode(token, _formats, _ci));
            }
            throw new NotSupportedException();
        }

        private RdlSyntaxNode ComposeStopAt()
        {
            var stopAtToken = Last;
            var token = Current;
            Consume(Current.TokenType);
            switch (token.TokenType)
            {
                case StatementType.Var:
                    throw new NotSupportedException("Not supported yet, sorry.");
                case StatementType.Word:
                    return new StopAtNode(stopAtToken, new DateTimeNode(token, _formats, _ci));
            }
            throw new NotSupportedException();
        }

        private RdlSyntaxNode ComposeWhere()
        {
            var node = ComposeEqualityOperators();
            while (IsQueryOperator(Current))
                switch (Current.TokenType)
                {
                    case StatementType.And:
                        Consume(StatementType.And);
                        node = new AndNode(node, ComposeEqualityOperators());
                        break;
                    case StatementType.Or:
                        Consume(StatementType.Or);
                        node = new OrNode(node, ComposeEqualityOperators());
                        break;
                    default:
                        throw new NotSupportedException();
                }
            return node;
        }

        private RdlSyntaxNode ComposeEqualityOperators()
        {
            var node = ComposeArithmeticOperators(Precendence.Level1);
            while (IsEqualityOperator(Current))
                switch (Current.TokenType)
                {
                    case StatementType.GreaterEqual:
                        Consume(StatementType.GreaterEqual);
                        node = new GreaterEqualNode(node, ComposeEqualityOperators());
                        break;
                    case StatementType.Greater:
                        Consume(StatementType.Greater);
                        node = new GreaterNode(node, ComposeEqualityOperators());
                        break;
                    case StatementType.LessEqual:
                        Consume(StatementType.LessEqual);
                        node = new LessEqualNode(node, ComposeEqualityOperators());
                        break;
                    case StatementType.Less:
                        Consume(StatementType.Less);
                        node = new LessNode(node, ComposeEqualityOperators());
                        break;
                    case StatementType.Equality:
                        Consume(StatementType.Equality);
                        node = new EqualityNode(node, ComposeEqualityOperators());
                        break;
                    case StatementType.Diff:
                        Consume(StatementType.Diff);
                        node = new DiffNode(node, ComposeEqualityOperators());
                        break;
                    case StatementType.Between:
                        node = new BetweenNode(ConsumeAndGetToken(), node,
                            ComposeAndSkip(f => f.ComposeArithmeticOperators(Precendence.Level1), StatementType.And),
                            ComposeArithmeticOperators(Precendence.Level1));
                        break;
                    case StatementType.Not:
                        Consume(StatementType.Not);
                        node = new NotNode(Current, node);
                        break;
                    default:
                        throw new NotSupportedException();
                }

            return node;
        }

        private RdlSyntaxNode ComposeArithmeticOperators(Precendence precendence)
        {
            RdlSyntaxNode node = null;
            switch (precendence)
            {
                case Precendence.Level1:
                {
                    node = ComposeArithmeticOperators(Precendence.Level2);
                    while (IsArithmeticOperator(Current, Precendence.Level1))
                        switch (Current.TokenType)
                        {
                            case StatementType.Star:
                                Consume(StatementType.Star);
                                node = new StarNode(node, ComposeBaseTypes());
                                break;
                            case StatementType.FSlash:
                                Consume(StatementType.FSlash);
                                node = new FSlashNode(node, ComposeBaseTypes());
                                break;
                            case StatementType.Mod:
                                Consume(StatementType.Mod);
                                node = new ModuloNode(node, ComposeBaseTypes());
                                break;
                            case StatementType.In:
                                Consume(StatementType.In);
                                node = new InNode(node, ComposeArgs());
                                break;
                            case StatementType.NotIn:
                                Consume(StatementType.NotIn);
                                node = new NotInNode(node, ComposeArgs());
                                break;
                        }
                    break;
                }
                case Precendence.Level2:
                {
                    node = ComposeArithmeticOperators(Precendence.Level3);
                    while (IsArithmeticOperator(Current, Precendence.Level2))
                        switch (Current.TokenType)
                        {
                            case StatementType.Plus:
                                Consume(StatementType.Plus);
                                node = new AddNode(node, ComposeBaseTypes());
                                break;
                            case StatementType.Hyphen:
                                Consume(StatementType.Hyphen);
                                node = new HyphenNode(node, ComposeBaseTypes());
                                break;
                        }
                    break;
                }
                case Precendence.Level3:
                    node = ComposeBaseTypes();
                    break;
            }
            return node;
        }

        private TNode SkipComposeSkip<TNode>(StatementType pStatenent, Func<RdlParser, TNode> parserAction,
            StatementType aStatement)
        {
            Consume(pStatenent);
            return ComposeAndSkip(parserAction, aStatement);
        }

        private TNode SkipAndCompose<TNode>(Func<RdlParser, TNode> parserAction, StatementType statement)
        {
            Consume(statement);
            return parserAction(this);
        }

        private TNode ComposeAndSkip<TNode>(Func<RdlParser, TNode> parserAction, StatementType statement)
        {
            var node = parserAction(this);
            Consume(statement);
            return node;
        }

        private RdlSyntaxNode ComposeBaseTypes()
        {
            switch (Current.TokenType)
            {
                case StatementType.Numeric:
                    return new NumericNode(ConsumeAndGetToken(StatementType.Numeric));
                case StatementType.Word:
                    return new WordNode(ConsumeAndGetToken(StatementType.Word));
                case StatementType.Var:
                    return new VarNode(ConsumeAndGetToken(StatementType.Var) as VarToken);
                case StatementType.Case:
                    return new CaseNode(ConsumeAndGetToken(), ComposeWhenThenNodes(),
                        ComposeAndSkip(f => ComposeElseNode(), StatementType.CaseEnd));
                case StatementType.Function:
                    var func = Current as FunctionToken;

                    if (func == null)
                        throw new ArgumentNullException();

                    Consume(StatementType.Function);

                    var args = ComposeArgs();
                    var argsTypes = args.Descendants.Select(f => f.ReturnType).ToArray();

                    MethodInfo registeredMethod;
                    if (_resolver.TryResolveMethod(func.Value, argsTypes, out registeredMethod))
                    {
                        var function = new RawFunctionNode(func, args, registeredMethod.ReturnType, _resolver.CanBeCached(registeredMethod));
                        var hashCodedFunction = function.Stringify().GetHashCode();

                        if (!FunctionCallOccurence.ContainsKey(hashCodedFunction))
                            FunctionCallOccurence.Add(hashCodedFunction, 1);
                        else
                            FunctionCallOccurence[hashCodedFunction] += 1;

                        return function;
                    }
                    throw new MethodNotFoundedException();
                case StatementType.LeftParenthesis:
                    return SkipComposeSkip(StatementType.LeftParenthesis, f => f.ComposeWhere(),
                        StatementType.RightParenthesis);
            }
            throw new NotSupportedException();
        }

        private ArgListNode ComposeArgs()
        {
            var args = new List<RdlSyntaxNode>();

            Consume(StatementType.LeftParenthesis);

            if (Current.TokenType != StatementType.RightParenthesis)
                do
                {
                    if (Current.TokenType == StatementType.Comma)
                        Consume(Current.TokenType);

                    args.Add(ComposeWhere());
                } while (Current.TokenType == StatementType.Comma);

            Consume(StatementType.RightParenthesis);

            return new ArgListNode(args);
        }

        private WhenThenNode[] ComposeWhenThenNodes()
        {
            var nodes = new List<WhenThenNode>();

            while (!IsElseNode(Current))
            {
                WhenNode when = null;
                ThenNode then = null;

                switch (Current.TokenType)
                {
                    case StatementType.When:
                        when = new WhenNode(ConsumeAndGetToken(), ComposeWhere());
                        goto case StatementType.Then;
                    case StatementType.Then:
                        then = new ThenNode(ConsumeAndGetToken(), ComposeWhere());
                        break;
                    default:
                        throw new NotSupportedException();
                }

                if (when == null || then == null)
                    throw new NullReferenceException();

                nodes.Add(new WhenThenNode(when, then));
            }

            return nodes.ToArray();
        }

        private ElseNode ComposeElseNode()
        {
            switch (Current.TokenType)
            {
                case StatementType.Else:
                    return new ElseNode(ConsumeAndGetToken(), ComposeEqualityOperators());
            }
            throw new NotSupportedException();
        }

        private Token ConsumeAndGetToken(StatementType expected)
        {
            var token = Current;
            Consume(expected);
            return token;
        }

        private bool IsElseNode(Token current)
            => current.TokenType == StatementType.Else;

        private Token ConsumeAndGetToken()
            => ConsumeAndGetToken(Current.TokenType);

        private bool IsArithmeticOperator(Token currentToken, Precendence precendence)
        {
            switch (precendence)
            {
                case Precendence.Level1:
                    return currentToken.TokenType == StatementType.Star ||
                           currentToken.TokenType == StatementType.FSlash ||
                           currentToken.TokenType == StatementType.Mod ||
                           currentToken.TokenType == StatementType.In ||
                           currentToken.TokenType == StatementType.NotIn;
                case Precendence.Level2:
                    return currentToken.TokenType == StatementType.Plus ||
                           currentToken.TokenType == StatementType.Hyphen;
                case Precendence.Level3:
                    return true;
            }

            return false;
        }

        private bool IsEqualityOperator(Token currentToken)
            => currentToken.TokenType == StatementType.Greater ||
               currentToken.TokenType == StatementType.GreaterEqual ||
               currentToken.TokenType == StatementType.Less ||
               currentToken.TokenType == StatementType.LessEqual ||
               currentToken.TokenType == StatementType.Equality ||
               currentToken.TokenType == StatementType.Not ||
               currentToken.TokenType == StatementType.Diff ||
               currentToken.TokenType == StatementType.Between;

        private static bool IsQueryOperator(Token currentToken)
            => currentToken.TokenType == StatementType.And || currentToken.TokenType == StatementType.Or;

        private void Consume(StatementType tokenType)
        {
            if (Current.TokenType.Equals(tokenType))
            {
                Lexer.Next();
                return;
            }
            throw new UnexpectedTokenException<StatementType>(Lexer.Position, Current);
        }

        private RdlSyntaxNode ComposeRepeat()
        {
            RepeatEveryNode node = null;
            var repeat = Last;
            switch (Current.TokenType)
            {
                case StatementType.Every:
                    var every = Current;
                    Consume(StatementType.Every);
                    if (Current.TokenType == StatementType.Numeric)
                    {
                        var numeric = Current;
                        Consume(StatementType.Numeric);
                        node = new NumericConsequentRepeatEveryNode(
                            new Token("repeat every", StatementType.Repeat,
                                new TQL.Core.Tokens.TextSpan(repeat.Span.Start, Current.Span.End - repeat.Span.Start)),
                            numeric as NumericToken,
                            Current as WordToken);
                    }
                    else
                    {
                        node = new RepeatEveryNode(Last, Current);
                    }
                    Consume(Current.TokenType);
                    break;
                default:
                    node = new RepeatEveryNode(repeat, Current);
                    break;
            }
            return node;
        }

        private enum Precendence : short
        {
            Level1,
            Level2,
            Level3
        }
    }
}