using System;
using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public abstract class BinaryNode : RdlSyntaxNode
    {
        protected BinaryNode(RdlSyntaxNode left, RdlSyntaxNode right)
        {
            Descendants = new[] { left, right };
        }

        public virtual RdlSyntaxNode Left => Descendants[0];

        public virtual RdlSyntaxNode Right => Descendants[1];

        public override bool IsLeaf => false;

        public override RdlSyntaxNode[] Descendants { get; }

        public override Token Token => null;

        public override TextSpan FullSpan => new TextSpan(Left.FullSpan.Start, Left.FullSpan.Length + Right.FullSpan.Length);

        public override Type ReturnType => Left.ReturnType;

        protected string ToString(string op) => string.Format("{0} {1} {2}", Left, op, Right);
    }
}
