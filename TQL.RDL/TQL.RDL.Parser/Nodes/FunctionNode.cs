using System;
using System.Linq;
using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class FunctionNode : RdlSyntaxNode
    {
        private ArgListNode _args;
        private FunctionToken _functionName;
        private Func<Type> _getReturnType;

        public FunctionNode(FunctionToken functionName, ArgListNode args, Func<Type> getReturnType = null)
            : base()
        {
            _functionName = functionName;
            _args = args;
            _getReturnType = getReturnType;
        }

        public override RdlSyntaxNode[] Descendants => _args.Descendants;

        public override TextSpan FullSpan
        {
            get
            {
                if (_args.Descendants.Length == 0)
                    return new TextSpan(_functionName.Span.Start, _functionName.Span.Length);
                return new TextSpan(_functionName.Span.Start, _args.Descendants[_args.Descendants.Length - 1].FullSpan.End - _functionName.Span.Start);
            }
        }

        public override bool IsLeaf => Descendants.Count() == 0;

        public override Token Token => _functionName;

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString() => _functionName.ToString() + '(' + string.Join<RdlSyntaxNode>(",", _args.Descendants) + ')';

        public string Name => Token.Value;

        public override Type ReturnType => _getReturnType();

        public ArgListNode Args => _args;
    }
}
