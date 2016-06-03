using System;
using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class RootScriptNode : RdlSyntaxNode
    {
        private RdlSyntaxNode[] parts;

        public RootScriptNode(RdlSyntaxNode[] parts)
        {
            this.parts = parts;
        }

        public override RdlSyntaxNode[] Descendants => parts;

        public override TextSpan FullSpan => new TextSpan(parts[0].FullSpan.Start, (parts[parts.Length - 1].FullSpan.End - parts[0].FullSpan.Start) + 1);

        public override bool IsLeaf => false;

        public override Type ReturnType => typeof(DateTimeOffset);

        public override Token Token => null;

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}
