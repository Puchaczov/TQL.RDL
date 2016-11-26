using RDL.Parser.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace TQL.RDL.Parser
{
    public class RdlMetadata
    {
        private Dictionary<string, List<MethodInfo>> methods;

        public RdlMetadata()
        {
            methods = new Dictionary<string, List<MethodInfo>>();
        }

        public Type GetReturnType(string function, Type[] args)
        {
            var method = GetMethod(function, args);
            return method.ReturnType;
        }

        public MethodInfo GetMethod(string name, Type[] methodArgs)
        {
            int index = -1;
            if(!HasMethod(name, methodArgs, out index))
                throw new Exception("Not matched");

            return methods[name][index];
        }

        public bool HasMethod(string name, Type[] methodArgs)
        {
            int index = 0;
            return HasMethod(name, methodArgs, out index);
        }

        private bool HasMethod(string name, Type[] methodArgs, out int index)
        {
            if (!this.methods.ContainsKey(name))
            {
                throw new Exception(name);
            }

            var methods = this.methods[name];

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

                index = i;
                return true;
            }

            index = -1;
            return false;
        }

        public void RegisterMethod(string name, MethodInfo methodInfo)
        {
            if (methods.ContainsKey(name))
                methods[name].Add(methodInfo);
            else
                methods.Add(name, new List<MethodInfo>() { methodInfo });
        }
    }
}
