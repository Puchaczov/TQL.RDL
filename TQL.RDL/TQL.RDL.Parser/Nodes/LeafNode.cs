using RDL.Parser.Tokens;
using TQL.Core.Tokens;

namespace RDL.Parser.Nodes
{
    public abstract class LeafNode : RdlSyntaxNode
    {
        private readonly Token _token;

        protected LeafNode(Token token)
        {
            _token = token;
        }

        public override TextSpan FullSpan => Token.Span.Clone();

        public override bool IsLeaf => true;

        public override Token Token => _token;

        public override string ToString() => _token.Value;
    }
}
