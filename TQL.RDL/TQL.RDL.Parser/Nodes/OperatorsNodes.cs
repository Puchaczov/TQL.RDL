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
        public AndNode(RdlSyntaxNode right, RdlSyntaxNode left)
            : base(left, right)
        { }

        public override string ToString() => ToString("and");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class OrNode : BinaryNode
    {
        public OrNode(RdlSyntaxNode right, RdlSyntaxNode left)
            : base(left, right)
        { }

        public override string ToString() => ToString("or");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class InNode : BinaryNode
    {
        public InNode(RdlSyntaxNode partOfDate, RdlSyntaxNode right)
            : base(partOfDate, right)
        { }

        public override string ToString() => string.Format("{0} {1} ({2})", Left.ToString(), "in", Right.ToString());
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class DiffNode : BinaryNode
    {
        public DiffNode(RdlSyntaxNode right, RdlSyntaxNode left)
            : base(left, right)
        { }

        public override string ToString() => ToString("<>");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class CommaNode : BinaryNode
    {
        public CommaNode(RdlSyntaxNode right, RdlSyntaxNode left)
            : base(left, right)
        { }

        public override string ToString() => string.Format("{0},{1}", Left.ToString(), Right.ToString());
        public override void Accept(INodeVisitor visitor) { }
    }

    public class EqualityNode : BinaryNode
    {
        public EqualityNode(RdlSyntaxNode right, RdlSyntaxNode left)
            : base(left, right)
        { }

        public override string ToString() => ToString("=");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class GreaterNode : BinaryNode
    {
        public GreaterNode(RdlSyntaxNode right, RdlSyntaxNode left)
            : base(left, right)
        { }

        public override string ToString() => ToString(">");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class GreaterEqualNode : BinaryNode
    {
        public GreaterEqualNode(RdlSyntaxNode right, RdlSyntaxNode left)
            : base(left, right)
        { }

        public override string ToString() => ToString(">=");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class LessNode : BinaryNode
    {
        public LessNode(RdlSyntaxNode right, RdlSyntaxNode left)
            : base(left, right)
        { }

        public override string ToString() => ToString("<");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class LessEqualNode : BinaryNode
    {
        public LessEqualNode(RdlSyntaxNode right, RdlSyntaxNode left)
            : base(left, right)
        { }

        public override string ToString() => ToString("<=");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class NotInNode : InNode
    {
        public NotInNode(RdlSyntaxNode partOfDate, RdlSyntaxNode right) 
            : base(partOfDate, right)
        { }

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString() => string.Format("not in {0}", base.Right);
    }

    public class AddNode : BinaryNode
    {
        public AddNode(RdlSyntaxNode right, RdlSyntaxNode left)
            : base(left, right)
        { }

        public override string ToString() => ToString("+");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class HyphenNode : BinaryNode
    {
        public HyphenNode(RdlSyntaxNode right, RdlSyntaxNode left)
            : base(left, right)
        { }

        public override string ToString() => ToString("-");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class ModuloNode : BinaryNode
    {
        public ModuloNode(RdlSyntaxNode right, RdlSyntaxNode left)
            : base(left, right)
        { }

        public override string ToString() => ToString("%");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class StarNode : BinaryNode
    {
        public StarNode(RdlSyntaxNode right, RdlSyntaxNode left)
            : base(left, right)
        { }

        public override string ToString() => ToString("*");
        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }

    public class FSlashNode : BinaryNode
    {
        public FSlashNode(RdlSyntaxNode right, RdlSyntaxNode left)
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
        { }

        public override Type ReturnType => Descendant.ReturnType;

        public override Token Token => token;

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class WhenNode : UnaryNode
    {
        private Token token;

        public WhenNode(Token token, RdlSyntaxNode when) 
            : base(when)
        { }

        public RdlSyntaxNode Expression => Descendant;

        public override Type ReturnType => Descendant.ReturnType;

        public override Token Token => token;

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class ThenNode : UnaryNode
    {
        private Token token;

        public ThenNode(Token token, RdlSyntaxNode then)
            : base(then)
        { }

        public override Type ReturnType => Descendant.ReturnType;

        public override Token Token => token;

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class WhenThenNode : BinaryNode
    {
        public WhenThenNode(RdlSyntaxNode when, RdlSyntaxNode then) : base(when, then)
        { }

        public WhenNode When => Descendants[0] as WhenNode;
        public ThenNode Then => Descendants[1] as ThenNode;

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class CaseNode : RdlSyntaxNode
    {
        public CaseNode(WhenThenNode[] nodes, ElseNode node)
        {
            this.whenThenExpressions = nodes;
            this.elseExpression = node;
        }

        private Token caseToken;

        private WhenThenNode[] whenThenExpressions;
        private ElseNode elseExpression;

        public override RdlSyntaxNode[] Descendants => new List<RdlSyntaxNode>(whenThenExpressions).Concat(new RdlSyntaxNode[1] { elseExpression }).ToArray();

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
    }
}
