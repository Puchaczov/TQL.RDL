using System;
using System.Linq;
using System.Reflection;

namespace RDL.Parser
{
    public class MethodDeclaration
    {
        public MethodDeclaration(MethodInfo methodInfo)
        {
            Method = methodInfo;
        }

        public string Name => Method.Name;

        public MethodInfo Method { get; }

        public Type[] Arguments => Method.GetParameters().Select(f => f.ParameterType).ToArray();
        public Type Return => Method.ReturnParameter.ParameterType;
    }
}