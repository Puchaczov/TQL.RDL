using System;
using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class RootScriptNode : RdlSyntaxNode
    {
        private readonly RdlSyntaxNode[] _parts;

        public RootScriptNode(RdlSyntaxNode[] parts)
        {
            _parts = parts;
        }

        public override RdlSyntaxNode[] Descendants => _parts;

        public override TextSpan FullSpan
            =>
                new TextSpan(_parts[0].FullSpan.Start,
                    _parts[_parts.Length - 1].FullSpan.End - _parts[0].FullSpan.Start + 1);

        public override bool IsLeaf => false;

        public override Type ReturnType => typeof(DateTimeOffset);

        public override Token Token => null;

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}