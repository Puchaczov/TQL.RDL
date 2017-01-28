using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RDL.Parser
{
    public interface IMethodDeclarationResolver
    {
        bool TryResolveMethod(string name, Type[] callArgs, out MethodInfo result);
    }
}
