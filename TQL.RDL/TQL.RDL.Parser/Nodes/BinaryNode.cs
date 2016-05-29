using System;
using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public abstract class BinaryNode : RdlSyntaxNode
    {
        private RdlSyntaxNode[] parts;

        public BinaryNode(RdlSyntaxNode left, RdlSyntaxNode right)
        {
            parts = new RdlSyntaxNode[] { left, right };
        }

        public virtual RdlSyntaxNode Left => Descendants[0];

        public virtual RdlSyntaxNode Right => Descendants[1];

        public override bool IsLeaf => false;

        public override RdlSyntaxNode[] Descendants => parts;

        public override Token Token => null;

        public override TextSpan FullSpan => new TextSpan(Left.FullSpan.Start, Left.FullSpan.Length + Right.FullSpan.Length);

        protected string ToString(string op) => string.Format("{0} {1} {2}", Left, op, Right);

        public override Type ReturnType => Left.ReturnType;
    }
}
