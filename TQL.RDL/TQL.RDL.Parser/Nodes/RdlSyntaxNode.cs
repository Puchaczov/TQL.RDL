using System;
using System.Diagnostics;
using System.Text;
using RDL.Parser.Tokens;
using TQL.Core.Syntax;

namespace RDL.Parser.Nodes
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public abstract class RdlSyntaxNode : SyntaxNodeBase<INodeVisitor, StatementType>
    {
        /// <summary>
        /// Get child items of node.
        /// </summary>
        public abstract RdlSyntaxNode[] Descendants { get; }

        /// <summary>
        /// Token assigned to node.
        /// </summary>
        public abstract Token Token { get; }

        /// <summary>
        /// Get return type associated to expression
        /// </summary>
        public abstract Type ReturnType { get; }

        /// <summary>
        /// Return flattened representation of node including it's Descendants.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            if (Descendants != null && Descendants.Length > 0)
            {
                for (var i = 0; i < Descendants.Length - 1; ++i)
                {
                    builder.Append(Descendants[i]);
                    builder.Append(' ');
                }
                builder.Append(Descendants[Descendants.Length - 1]);
            }
            return builder.ToString();
        }
    }
}
