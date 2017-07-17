using System;
using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
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
                if (_token.Value != "current")
                    return typeof(long);
                return typeof(DateTimeOffset);
            }
        }

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString() => _token.ToString();
    }
}