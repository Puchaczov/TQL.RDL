using System;
using TQL.Core.Tokens;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class AndNode : BinaryNode
    {
        public AndNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override string ToString() => ToString("and");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override Type ReturnType => typeof(bool);
    }

    public class OrNode : BinaryNode
    {
        public OrNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override string ToString() => ToString("or");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override Type ReturnType => typeof(bool);
    }

    public class InNode : BinaryNode
    {
        public InNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override Type ReturnType => typeof(Boolean);

        public override string ToString() => string.Format("{0} {1} ({2})", Left.ToString(), "in", Right.ToString());
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class DiffNode : BinaryNode
    {
        public DiffNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override string ToString() => ToString("<>");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override Type ReturnType => typeof(bool);
    }

    public class CommaNode : BinaryNode
    {
        public CommaNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override string ToString() => string.Format("{0},{1}", Left.ToString(), Right.ToString());
        public override void Accept(INodeVisitor visitor) { }
    }

    public class EqualityNode : BinaryNode
    {
        public EqualityNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override string ToString() => ToString("=");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override Type ReturnType => typeof(bool);
    }

    public class GreaterNode : BinaryNode
    {
        public GreaterNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override string ToString() => ToString(">");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override Type ReturnType => typeof(bool);
    }

    public class GreaterEqualNode : BinaryNode
    {
        public GreaterEqualNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override string ToString() => ToString(">=");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override Type ReturnType => typeof(bool);
    }

    public class LessNode : BinaryNode
    {
        public LessNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override string ToString() => ToString("<");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override Type ReturnType => typeof(bool);
    }

    public class LessEqualNode : BinaryNode
    {
        public LessEqualNode(RdlSyntaxNode left, RdlSyntaxNode right)
            : base(left, right)
        { }

        public override string ToString() => ToString("<=");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override Type ReturnType => typeof(bool);
    }

    public class NotInNode : InNode
    {
        public NotInNode(RdlSyntaxNode left, RdlSyntaxNode right) 
            : base(left, right)
        { }

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString() => string.Format("not in {0}", Right);

        public override Type ReturnType => typeof(bool);
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
        private RdlSyntaxNode[] _args;

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
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < (_args.Length - 1); ++i)
            {
                builder.Append(_args[i].ToString());
                builder.Append(", ");
            }
            if(_args.Length > 0)
                builder.Append(_args[_args.Length - 1].ToString());
            return builder.ToString();
        }
    }

    public class VarNode : LeafNode
    {
        private VarToken _token;

        public VarNode(VarToken token)
            : base(token)
        {
            _token = token;
        }

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[0];

        public override TextSpan FullSpan => _token.Span;

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString() => _token.ToString();

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
    }

    public class ElseNode : UnaryNode
    {
        private Token _token;

        public ElseNode(Token token, RdlSyntaxNode node)
            : base(node)
        {
            _token = token;
        }

        public override Type ReturnType => Descendant.ReturnType;

        public override Token Token => _token;

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
        private Token _token;

        public WhenNode(Token token, RdlSyntaxNode when) 
            : base(when)
        {
            _token = token;
        }

        public RdlSyntaxNode Expression => Descendant;

        public override Type ReturnType => Descendant.ReturnType;

        public override Token Token => _token;

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
        private Token _token;

        public ThenNode(Token token, RdlSyntaxNode then)
            : base(then)
        { }

        public override Type ReturnType => Descendant.ReturnType;

        public override Token Token => _token;

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
        private RdlSyntaxNode[] _descs;

        public WhenThenNode(RdlSyntaxNode when, RdlSyntaxNode then) : base(when, then)
        {
            _descs = base.Descendants.ToArray();

            When.SetParent(this);
            Then.SetParent(this);

        }

        public WhenNode When => Descendants[0] as WhenNode;
        public ThenNode Then => Descendants[1] as ThenNode;

        public override RdlSyntaxNode[] Descendants => _descs;

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

        public void SetParent(CaseNode parent)
        {
            Parent = parent;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString() => string.Format("{0} {1}", When, Then);

        public override Type ReturnType => Then.ReturnType;
    }

    public class CaseNode : RdlSyntaxNode
    {
        private RdlSyntaxNode[] _descs;

        public CaseNode(Token caseToken, WhenThenNode[] nodes, ElseNode node)
        {
            node.SetParent(this);

            _whenThenExpressions = nodes;
            _elseExpression = node;

            foreach(var item in nodes)
            {
                item.SetParent(this);
            }

            _caseToken = caseToken;

            _descs = _whenThenExpressions.Concat(new RdlSyntaxNode[1] { _elseExpression }).ToArray();
        }

        private Token _caseToken;

        private WhenThenNode[] _whenThenExpressions;
        private ElseNode _elseExpression;

        public override RdlSyntaxNode[] Descendants => _descs;

        public RdlSyntaxNode[] WhenThenExpressions => _whenThenExpressions;

        public override TextSpan FullSpan => new TextSpan(_caseToken.Span.Start, _elseExpression.FullSpan.End - _caseToken.Span.Start);

        public override bool IsLeaf => false;

        public override Type ReturnType => _whenThenExpressions[0].ReturnType;

        public WhenThenNode[] Expressions => _whenThenExpressions;
        public ElseNode Else => _elseExpression;

        public override Token Token => _caseToken;

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override int GetHashCode() => ToString().GetHashCode();
    }
}
