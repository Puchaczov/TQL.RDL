using System;
using System.Reflection;

namespace TQL.RDL.Parser
{
    public interface IMethodDeclarationResolver
    {
        /// <summary>
        /// Try match existing method with passed name and arguments.
        /// </summary>
        /// <param name="name">Method name.</param>
        /// <param name="callArgs">Types of passed arguments.</param>
        /// <param name="result">Matched method.</param>
        /// <returns>True if method were matched, else false.</returns>
        bool TryResolveMethod(string name, Type[] callArgs, out MethodInfo result);

        /// <summary>
        /// Determine if passed method can be cached.
        /// </summary>
        /// <returns>True if method call can be cached, otherwise false.</returns>
        bool CanBeCached(MethodInfo method);
    }
}