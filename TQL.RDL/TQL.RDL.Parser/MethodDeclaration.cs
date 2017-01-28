using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RDL.Parser.Helpers;

namespace RDL.Parser
{
    public class MethodDeclaration
    {
        private readonly MethodInfo _methodInfo;

        public MethodDeclaration(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
        }

        public string Name => _methodInfo.Name;

        public MethodInfo Method => _methodInfo;
        public Type[] Arguments => _methodInfo.GetParameters().Select(f => f.ParameterType).ToArray();
        public Type Return => _methodInfo.ReturnParameter.ParameterType;
    }
}
