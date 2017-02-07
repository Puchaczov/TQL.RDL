using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDL.Parser.Tokens;
using TQL.Core.Tokens;

namespace RDL.Parser.Nodes
{
    public class AndNode : BinaryNode
    {
        public AndNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override Type ReturnType => typeof(bool);

        public override string ToString() => ToString("and");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class OrNode : BinaryNode
    {
        public OrNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override Type ReturnType => typeof(bool);

        public override string ToString() => ToString("or");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class InNode : BinaryNode
    {
        public InNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override Type ReturnType => typeof(bool);

        public override string ToString() => string.Format("{0} {1} ({2})", Left, "in", Right);
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class DiffNode : BinaryNode
    {
        public DiffNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override Type ReturnType => typeof(bool);

        public override string ToString() => ToString("<>");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class CommaNode : BinaryNode
    {
        public CommaNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override string ToString() => string.Format("{0},{1}", Left, Right);
        public override void Accept(INodeVisitor visitor) { }
    }

    public class EqualityNode : BinaryNode
    {
        public EqualityNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override Type ReturnType => typeof(bool);

        public override string ToString() => ToString("=");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class GreaterNode : BinaryNode
    {
        public GreaterNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override Type ReturnType => typeof(bool);

        public override string ToString() => ToString(">");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class GreaterEqualNode : BinaryNode
    {
        public GreaterEqualNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override Type ReturnType => typeof(bool);

        public override string ToString() => ToString(">=");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class LessNode : BinaryNode
    {
        public LessNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override Type ReturnType => typeof(bool);

        public override string ToString() => ToString("<");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class LessEqualNode : BinaryNode
    {
        public LessEqualNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override Type ReturnType => typeof(bool);

        public override string ToString() => ToString("<=");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class NotInNode : InNode
    {
        public NotInNode(RdlSyntaxNode left, RdlSyntaxNode right) 
            : base(left, right)
        { }

        public override Type ReturnType => typeof(bool);

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString() => string.Format("not in {0}", Right);
    }

    public class AddNode : BinaryNode
    {
        public AddNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override string ToString() => ToString("+");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class HyphenNode : BinaryNode
    {
        public HyphenNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override string ToString() => ToString("-");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class ModuloNode : BinaryNode
    {
        public ModuloNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override string ToString() => ToString("%");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class StarNode : BinaryNode
    {
        public StarNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override string ToString() => ToString("*");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class FSlashNode : BinaryNode
    {
        public FSlashNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override string ToString() => ToString("/");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class ArgListNode : RdlSyntaxNode
    {
        private readonly RdlSyntaxNode[] _args;

        public ArgListNode(IEnumerable<RdlSyntaxNode> args)
        {
            _args = args.Select(f => f).ToArray();
        }

        public override RdlSyntaxNode[] Descendants => _args;

        public override TextSpan FullSpan => new TextSpan(_args[0].FullSpan.Start, _args[_args.Length - 1].FullSpan.End - _args[0].FullSpan.Start);

        public override bool IsLeaf => false;

        public override Type ReturnType => typeof(ArgListNode);

        public override Token Token => null;

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString()
        {
            var builder = new StringBuilder();
            for (var i = 0; i < _args.Length - 1; ++i)
            {
                builder.Append(_args[i]);
                builder.Append(", ");
            }
            if(_args.Length > 0)
                builder.Append(_args[_args.Length - 1]);
            return builder.ToString();
        }
    }

    public class VarNode : LeafNode
    {
        private readonly VarToken _token;

        public VarNode(VarToken token)
            : base(token)
        {
            _token = token;
        }

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[0];

        public override TextSpan FullSpan => _token.Span;

        public string Value => _token.Value;

        public override Type ReturnType
        {
            get
            {
                //TO DO: much better implementation of this: register variable in global metadata and use it here
                if(_token.Value != "current")
                {
                    return typeof(long);
                }
                return typeof(DateTimeOffset);
            }
        }

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString() => _token.ToString();
    }

    public class ElseNode : UnaryNode
    {
        public ElseNode(Token token, RdlSyntaxNode node)
            : base(node)
        {
            Token = token;
        }

        public override Type ReturnType => Descendant.ReturnType;

        public override Token Token { get; }

        public CaseNode Parent { get; private set; }

        public void SetParent(CaseNode parent)
        {
            Parent = parent;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString() => string.Format("else {0}", Descendants[0]);
    }

    public class WhenNode : UnaryNode
    {
        public WhenNode(Token token, RdlSyntaxNode when) 
            : base(when)
        {
            Token = token;
        }

        public RdlSyntaxNode Expression => Descendant;

        public override Type ReturnType => Descendant.ReturnType;

        public override Token Token { get; }

        public WhenThenNode Parent { get; private set; }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void SetParent(WhenThenNode node)
        {
            Parent = node;
        }

        public override string ToString() => string.Format("when {0}", Expression);
    }

    public class ThenNode : UnaryNode
    {
        public ThenNode(Token token, RdlSyntaxNode then)
            : base(then)
        {
            Token = token;
        }

        public override Type ReturnType => Descendant.ReturnType;

        public override Token Token { get; }

        public WhenThenNode Parent { get; private set; }

        public void SetParent(WhenThenNode parent)
        {
            Parent = parent;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString() => string.Format("then {0}", Descendants[0]);
    }

    public class WhenThenNode : BinaryNode
    {
        public WhenThenNode(RdlSyntaxNode when, RdlSyntaxNode then) : base(when, then)
        {
            Descendants = base.Descendants.ToArray();

            When.SetParent(this);
            Then.SetParent(this);

        }

        public WhenNode When => Descendants[0] as WhenNode;
        public ThenNode Then => Descendants[1] as ThenNode;

        public override RdlSyntaxNode[] Descendants { get; }

        public CaseNode Parent { get; private set; }

        public int ArrayOrder {
            get
            {
                for(int i = 0, j = Parent.Descendants.Count(); i < j; ++i)
                {
                    if (Parent.Descendants[i] == this)
                        return i;
                }
                return -1;
            }
        }

        public int WhenThenCount => Parent.Descendants.Count() - 1;

        public override Type ReturnType => Then.ReturnType;

        public void SetParent(CaseNode parent)
        {
            Parent = parent;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString() => string.Format("{0} {1}", When, Then);
    }

    public class CaseNode : RdlSyntaxNode
    {
        private readonly Token _caseToken;

        public CaseNode(Token caseToken, WhenThenNode[] nodes, ElseNode node)
        {
            node.SetParent(this);

            Expressions = nodes;
            Else = node;

            foreach(var item in nodes)
            {
                item.SetParent(this);
            }

            _caseToken = caseToken;

            Descendants = Expressions.Concat(new RdlSyntaxNode[1] { Else }).ToArray();
        }

        public override RdlSyntaxNode[] Descendants { get; }

        public RdlSyntaxNode[] WhenThenExpressions => Expressions;

        public override TextSpan FullSpan => new TextSpan(_caseToken.Span.Start, Else.FullSpan.End - _caseToken.Span.Start);

        public override bool IsLeaf => false;

        public override Type ReturnType => Expressions[0].ReturnType;

        public WhenThenNode[] Expressions { get; }

        public ElseNode Else { get; }

        public override Token Token => _caseToken;

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override int GetHashCode() => ToString().GetHashCode();
    }

    public class BetweenNode : RdlSyntaxNode
    {
        private readonly Token _betweenToken;

        public BetweenNode(Token betweenToken, RdlSyntaxNode exp, RdlSyntaxNode minNode, RdlSyntaxNode maxNode)
        {
            Descendants = new[] {exp, minNode, maxNode};
            _betweenToken = betweenToken;
        }

        public override RdlSyntaxNode[] Descendants { get; }

        public RdlSyntaxNode Expression => Descendants[0];

        public RdlSyntaxNode Min => Descendants[1];

        public RdlSyntaxNode Max => Descendants[2];

        public override TextSpan FullSpan => new TextSpan(Expression.FullSpan.Start, Max.FullSpan.End - Expression.FullSpan.Start);

        public override bool IsLeaf => false;

        public override Type ReturnType => typeof(Boolean);

        public override Token Token => _betweenToken;

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString() => $"{Expression} between {Min} and {Max}";
    }
}
