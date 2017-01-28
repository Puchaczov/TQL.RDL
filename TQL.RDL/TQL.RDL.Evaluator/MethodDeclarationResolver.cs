using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RDL.Parser;
using RDL.Parser.Helpers;
using TQL.RDL.Evaluator.Attributes;

namespace TQL.RDL.Evaluator
{
    public class MethodDeclarationResolver : IMethodDeclarationResolver
    {
        private readonly RdlMetadata _metadata;

        /// <summary>
        /// Initialize object.
        /// </summary>
        /// <param name="metadata">Metadatas.</param>
        public MethodDeclarationResolver(RdlMetadata metadata)
        {
            this._metadata = metadata;
        }

        /// <summary>
        /// Determine which method should be called for passed arguments.
        /// </summary>
        /// <param name="name">Method to found.</param>
        /// <param name="callArgs">Types of function arguments.</param>
        /// <param name="result">Founded method.</param>
        /// <returns>true if method is usable for that call, else false.</returns>
        public bool TryResolveMethod(string name, Type[] callArgs, out MethodInfo result)
            => _metadata.TryGetMethod(name, callArgs, out result);
    }
}
