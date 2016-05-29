using System;
using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class FunctionNode : RdlSyntaxNode
    {
        private ArgListNode args;
        private FunctionToken functionName;

        public FunctionNode(FunctionToken functionName, ArgListNode args)
            : base()
        {
            this.functionName = functionName;
            this.args = args;
        }

        public override RdlSyntaxNode[] Descendants => args.Descendants;

        public override TextSpan FullSpan => new TextSpan(functionName.Span.Start, args.Descendants[args.Descendants.Length - 1].FullSpan.End - functionName.Span.Start);

        public override bool IsLeaf => false;

        public override Token Token => functionName;

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString() => functionName.ToString() + '(' + string.Join<RdlSyntaxNode>(",", args.Descendants) + ')';

        public string Name => Token.Value;

        public override Type ReturnType => typeof(Boolean);

        public ArgListNode Args => args;
    }
}
