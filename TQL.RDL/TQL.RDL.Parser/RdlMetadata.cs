using System;
using System.Collections.Generic;
using System.Reflection;
using RDL.Parser.Exceptions;
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
                throw new MethodNotFoundedException();

            return _methods[name][index];
        }

        public bool HasMethod(string name, Type[] methodArgs)
        {
            int index;
            return HasMethod(name, methodArgs, out index);
        }

        private bool HasMethod(string name, IReadOnlyList<Type> methodArgs, out int index)
        {
            if (!_methods.ContainsKey(name))
            {
                index = -1;
                return false;
            }

            var methods = _methods[name];

            for (int i = 0, j = methods.Count; i < j; ++i)
            {
                var methodInfo = methods[i];
                var parameters = methodInfo.GetParameters();
                var optionalParametersCount = parameters.OptionalParameters();

                //Wrong amount of argument's. That's not our function.
                if (HasMoreArgumentsThanMethodDefinitionContains(methodArgs, parameters) || 
                    !CanUseSomeArgumentsAsDefaultParameters(methodArgs, parameters, optionalParametersCount))
                    continue;

                var hasMatchedArgTypes = true;
                for (int f = 0, g = methodArgs.Count; f < g; ++f)
                {
                    //When constant value, it won't be nullable<type> but type.
                    //So it is possible to call function with such value.
                    if (parameters[f].ParameterType.GetUnderlyingNullable() == methodArgs[f].GetUnderlyingNullable())
                        continue;
                    hasMatchedArgTypes = false;
                    break;
                }

                if (!hasMatchedArgTypes)
                    continue;

                index = i;
                return true;
            }

            index = -1;
            return false;
        }

        private static bool CanUseSomeArgumentsAsDefaultParameters(IReadOnlyCollection<Type> methodArgs, IReadOnlyCollection<ParameterInfo> parameters, int optionalParametersCount)
        {
            return ((methodArgs.Count >= (parameters.Count - optionalParametersCount)) && methodArgs.Count <= parameters.Count);
        }

        private static bool HasMoreArgumentsThanMethodDefinitionContains(IReadOnlyList<Type> methodArgs, ParameterInfo[] parameters) => methodArgs.Count > parameters.Length;

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
