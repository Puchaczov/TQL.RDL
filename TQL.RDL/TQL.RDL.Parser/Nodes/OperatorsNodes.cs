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

        public override string ToString() => string.Format("not in {0}", base.Right);

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
        private RdlSyntaxNode[] args;

        public ArgListNode(IEnumerable<RdlSyntaxNode> args)
        {
            this.args = args.Select(f => f).ToArray();
        }

        public override RdlSyntaxNode[] Descendants => args;

        public override TextSpan FullSpan => new TextSpan(args[0].FullSpan.Start, args[args.Length - 1].FullSpan.End - args[0].FullSpan.Start);

        public override bool IsLeaf => false;

        public override Type ReturnType => typeof(ArgListNode);

        public override Token Token => null;

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < (args.Length - 1); ++i)
            {
                builder.Append(args[i].ToString());
                builder.Append(", ");
            }
            if(args.Length > 0)
                builder.Append(args[args.Length - 1].ToString());
            return builder.ToString();
        }
    }

    public class VarNode : LeafNode
    {
        private VarToken token;

        public VarNode(VarToken token)
            : base(token)
        {
            this.token = token;
        }

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[0];

        public override TextSpan FullSpan => token.Span;

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString() => token.ToString();

        public string Value => token.Value;

        public override Type ReturnType
        {
            get
            {
                //TO DO: much better implementation of this: register variable in global metadata and use it here
                if(token.Value != "current")
                {
                    return typeof(long);
                }
                return typeof(DateTimeOffset);
            }
        }
    }

    public class ElseNode : UnaryNode
    {
        private Token token;

        public ElseNode(Token token, RdlSyntaxNode node)
            : base(node)
        {
            this.token = token;
        }

        public override Type ReturnType => Descendant.ReturnType;

        public override Token Token => token;

        public CaseNode Parent { get; private set; }

        public void SetParent(CaseNode parent)
        {
            this.Parent = parent;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString() => string.Format("else {0}", Descendants[0]);
    }

    public class WhenNode : UnaryNode
    {
        private Token token;

        public WhenNode(Token token, RdlSyntaxNode when) 
            : base(when)
        {
            this.token = token;
        }

        public RdlSyntaxNode Expression => Descendant;

        public override Type ReturnType => Descendant.ReturnType;

        public override Token Token => token;

        public WhenThenNode Parent { get; private set; }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void SetParent(WhenThenNode node)
        {
            this.Parent = node;
        }

        public override string ToString() => string.Format("when {0}", Expression);
    }

    public class ThenNode : UnaryNode
    {
        private Token token;

        public ThenNode(Token token, RdlSyntaxNode then)
            : base(then)
        { }

        public override Type ReturnType => Descendant.ReturnType;

        public override Token Token => token;

        public WhenThenNode Parent { get; private set; }

        public void SetParent(WhenThenNode parent)
        {
            this.Parent = parent;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString() => string.Format("then {0}", Descendants[0]);
    }

    public class WhenThenNode : BinaryNode
    {
        private RdlSyntaxNode[] descs;

        public WhenThenNode(RdlSyntaxNode when, RdlSyntaxNode then) : base(when, then)
        {
            this.descs = base.Descendants.ToArray();

            When.SetParent(this);
            Then.SetParent(this);

        }

        public WhenNode When => Descendants[0] as WhenNode;
        public ThenNode Then => Descendants[1] as ThenNode;

        public override RdlSyntaxNode[] Descendants => descs;

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
            this.Parent = parent;
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
        private RdlSyntaxNode[] descs;

        public CaseNode(Token caseToken, WhenThenNode[] nodes, ElseNode node)
        {
            node.SetParent(this);

            this.whenThenExpressions = nodes;
            this.elseExpression = node;

            foreach(var item in nodes)
            {
                item.SetParent(this);
            }

            this.caseToken = caseToken;

            this.descs = whenThenExpressions.Concat(new RdlSyntaxNode[1] { elseExpression }).ToArray();
        }

        private Token caseToken;

        private WhenThenNode[] whenThenExpressions;
        private ElseNode elseExpression;

        public override RdlSyntaxNode[] Descendants => descs;

        public RdlSyntaxNode[] WhenThenExpressions => whenThenExpressions;

        public override TextSpan FullSpan => new TextSpan(caseToken.Span.Start, elseExpression.FullSpan.End - caseToken.Span.Start);

        public override bool IsLeaf => false;

        public override Type ReturnType => whenThenExpressions[0].ReturnType;

        public WhenThenNode[] Expressions => whenThenExpressions;
        public ElseNode Else => elseExpression;

        public override Token Token => caseToken;

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override int GetHashCode() => ToString().GetHashCode();
    }
}
