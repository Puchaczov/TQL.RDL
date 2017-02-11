using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDL.Parser.Tokens;
using TQL.Core.Tokens;

namespace RDL.Parser.Nodes
{
    public class ArgListNode : RdlSyntaxNode
    {
        private readonly RdlSyntaxNode[] _args;

        public ArgListNode(IEnumerable<RdlSyntaxNode> args)
        {
            _args = args.Select(f => f).ToArray();
        }

        public override RdlSyntaxNode[] Descendants => _args;

        public override TextSpan FullSpan
            => new TextSpan(_args[0].FullSpan.Start, _args[_args.Length - 1].FullSpan.End - _args[0].FullSpan.Start);

        public override bool IsLeaf => false;

        public override Type ReturnType => typeof(ArgListNode);

        public override Token Token => null;

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString()
        {
            var builder = new StringBuilder();
            for (var i = 0; i < _args.Length - 1; ++i)
            {
                builder.Append(_args[i]);
                builder.Append(", ");
            }
            if (_args.Length > 0)
                builder.Append(_args[_args.Length - 1]);
            return builder.ToString();
        }
    }
}