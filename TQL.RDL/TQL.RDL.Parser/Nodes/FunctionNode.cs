using System;
using System.Linq;
using RDL.Parser.Tokens;
using TQL.Core.Tokens;

namespace RDL.Parser.Nodes
{
    public class FunctionNode : RdlSyntaxNode
    {
        private readonly FunctionToken _functionName;
        private readonly Type _returnType;

        public FunctionNode(FunctionToken functionName, ArgListNode args, Type returnType)
        {
            _functionName = functionName;
            Args = args;
            _returnType = returnType;
        }

        public override RdlSyntaxNode[] Descendants => Args.Descendants;

        public override TextSpan FullSpan
        {
            get
            {
                if (Args.Descendants.Length == 0)
                    return new TextSpan(_functionName.Span.Start, _functionName.Span.Length);
                return new TextSpan(_functionName.Span.Start, Args.Descendants[Args.Descendants.Length - 1].FullSpan.End - _functionName.Span.Start);
            }
        }

        public override bool IsLeaf => !Descendants.Any();

        public override Token Token => _functionName;

        public string Name => Token.Value;

        public override Type ReturnType => _returnType;

        public ArgListNode Args { get; }

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString() => _functionName.ToString() + '(' + string.Join<RdlSyntaxNode>(",", Args.Descendants) + ')';
    }
}
