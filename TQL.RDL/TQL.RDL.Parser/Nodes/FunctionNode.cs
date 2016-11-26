using System;
using System.Linq;
using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class FunctionNode : RdlSyntaxNode
    {
        private ArgListNode args;
        private FunctionToken functionName;
        private Func<Type> getReturnType;

        public FunctionNode(FunctionToken functionName, ArgListNode args, Func<Type> getReturnType = null)
            : base()
        {
            this.functionName = functionName;
            this.args = args;
            this.getReturnType = getReturnType;
        }

        public override RdlSyntaxNode[] Descendants => args.Descendants;

        public override TextSpan FullSpan
        {
            get
            {
                if (args.Descendants.Length == 0)
                    return new TextSpan(functionName.Span.Start, functionName.Span.Length);
                return new TextSpan(functionName.Span.Start, args.Descendants[args.Descendants.Length - 1].FullSpan.End - functionName.Span.Start);
            }
        }

        public override bool IsLeaf => Descendants.Count() == 0;

        public override Token Token => functionName;

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString() => functionName.ToString() + '(' + string.Join<RdlSyntaxNode>(",", args.Descendants) + ')';

        public string Name => Token.Value;

        public override Type ReturnType => getReturnType();

        public ArgListNode Args => args;
    }
}
