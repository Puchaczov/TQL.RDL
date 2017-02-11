using System;
using System.Reflection;

namespace RDL.Parser
{
    public interface IMethodDeclarationResolver
    {
        bool TryResolveMethod(string name, Type[] callArgs, out MethodInfo result);
    }
}