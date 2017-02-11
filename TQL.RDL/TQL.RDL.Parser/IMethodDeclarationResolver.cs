using System;
using System.Reflection;

namespace RDL.Parser
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
    }
}