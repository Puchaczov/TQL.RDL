using System;
using System.Linq;

namespace TQL.RDL.Parser.Nodes
{
    public class WhenThenNode : BinaryNode
    {
        public WhenThenNode(RdlSyntaxNode when, RdlSyntaxNode then) : base(when, then)
        {
            Descendants = base.Descendants.ToArray();

            When.SetParent(this);
            Then.SetParent(this);
        }

        private WhenNode When => Descendants[0] as WhenNode;
        private ThenNode Then => Descendants[1] as ThenNode;

        public override RdlSyntaxNode[] Descendants { get; }

        public CaseNode Parent { get; private set; }

        public int ArrayOrder
        {
            get
            {
                for (int i = 0, j = Parent.Descendants.Length; i < j; ++i)
                    if (Parent.Descendants[i] == this)
                        return i;
                return -1;
            }
        }

        public int WhenThenCount => Parent.Descendants.Length - 1;

        public override Type ReturnType => Then.ReturnType;

        public void SetParent(CaseNode parent)
        {
            Parent = parent;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString() => $"{When} {Then}";
    }
}