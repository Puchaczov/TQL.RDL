using System;
using System.Collections.Generic;
using System.Reflection;
using RDL.Parser.Helpers;

namespace TQL.RDL.Parser
{
    public class RdlMetadata
    {
        private readonly Dictionary<string, List<MethodInfo>> _methods;

        public RdlMetadata()
        {
            _methods = new Dictionary<string, List<MethodInfo>>();
        }

        public Type GetReturnType(string function, Type[] args)
        {
            var method = GetMethod(function, args);
            return method.ReturnType;
        }

        public MethodInfo GetMethod(string name, Type[] methodArgs)
        {
            var index = -1;
            if(!HasMethod(name, methodArgs, out index))
                throw new Exception("Not matched");

            return _methods[name][index];
        }

        public bool HasMethod(string name, Type[] methodArgs)
        {
            var index = 0;
            return HasMethod(name, methodArgs, out index);
        }

        private bool HasMethod(string name, Type[] methodArgs, out int index)
        {
            if (!_methods.ContainsKey(name))
            {
                throw new Exception(name);
            }

            var methods = _methods[name];

            for (int i = 0, j = methods.Count; i < j; ++i)
            {
                var methodInfo = methods[i];
                var parameters = methodInfo.GetParameters();

                if (methodArgs.Length != parameters.Length)
                    continue;

                var hasMatchedArgTypes = true;
                for (var f = 0; f < parameters.Length; ++f)
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
            if (_methods.ContainsKey(name))
                _methods[name].Add(methodInfo);
            else
                _methods.Add(name, new List<MethodInfo> { methodInfo });
        }

        public void RegisterMethods<TType>(string methodName)
        {
            var type = typeof(TType);
            var typeInfo = type.GetTypeInfo();
            var methods = typeInfo.GetDeclaredMethods(methodName);

            foreach (var m in methods)
                RegisterMethod(m.Name, m);
        }
    }
}
