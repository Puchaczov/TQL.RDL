using RDL.Parser.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace TQL.RDL.Parser
{
    public static class GlobalMetadata
    {
        private static Dictionary<string, List<MethodInfo>> methods;

        static GlobalMetadata()
        {
            methods = new Dictionary<string, List<MethodInfo>>();
        }

        public static Type GetReturnType(string function, Type[] args)
        {
            var method = GetMethod(function, args);
            return method.ReturnType;
        }

        public static MethodInfo GetMethod(string name, Type[] methodArgs)
        {
            if (!GlobalMetadata.methods.ContainsKey(name))
            {
                throw new Exception(name);
            }

            var methods = GlobalMetadata.methods[name];

            for (int i = 0, j = methods.Count; i < j; ++i)
            {
                var methodInfo = methods[i];
                var parameters = methodInfo.GetParameters();

                if (methodArgs.Length != parameters.Length)
                    continue;

                bool hasMatchedArgTypes = true;
                for (int f = 0; f < parameters.Length; ++f)
                {
                    //When constant value, it won't be nullable<type> but type.
                    //So it is possible to call function with such value.
                    if (parameters[f].ParameterType.GetUnderlyingNullable() != methodArgs[f].GetUnderlyingNullable()) //long? != long
                    {
                        hasMatchedArgTypes = false;
                        break;
                    }
                }

                if (!hasMatchedArgTypes)
                    continue;

                return methods[i];
            }

            throw new Exception("Not matched");
        }

        public static void RegisterMethod(string name, MethodInfo methodInfo)
        {
            if (methods.ContainsKey(name))
                methods[name].Add(methodInfo);
            else
                methods.Add(name, new List<MethodInfo>() { methodInfo });
        }
    }
}
