﻿using System;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class NotNode : UnaryNode
    {
        public NotNode(Token token, RdlSyntaxNode node) : base(node)
        {
            Token = token;
        }

        public override Token Token { get; }
        public override Type ReturnType => typeof(bool);

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}