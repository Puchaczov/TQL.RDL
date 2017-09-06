﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TQL.RDL.Evaluator.Attributes;
using TQL.RDL.Parser;
using TQL.RDL.Parser.Exceptions;
using TQL.RDL.Parser.Helpers;

namespace TQL.RDL.Evaluator
{
    public class RdlMetadata
    {
        private static readonly Dictionary<Type, Type[]> TypeCompatibilityTable;
        private readonly Dictionary<string, List<MethodInfo>> _methods;

        static RdlMetadata()
        {
            TypeCompatibilityTable = new Dictionary<Type, Type[]>
            {
                {typeof(bool), new[] {typeof(bool)}},
                {typeof(short), new[] {typeof(short), typeof(bool)}},
                {typeof(int), new[] {typeof(int), typeof(short), typeof(bool)}},
                {typeof(long), new[] {typeof(long), typeof(int), typeof(short), typeof(bool)}},
                {typeof(DateTimeOffset), new[] {typeof(DateTimeOffset)}},
                {typeof(string), new[] {typeof(string)}}
            };
        }

        /// <summary>
        ///     Initialize object.
        /// </summary>
        public RdlMetadata()
        {
            _methods = new Dictionary<string, List<MethodInfo>>();
        }

        /// <summary>
        ///     Gets retrun type of function.
        /// </summary>
        /// <param name="function">Function name</param>
        /// <param name="args">Function args</param>
        /// <returns>Returned type of function.</returns>
        public Type GetReturnType(string function, Type[] args)
        {
            var method = GetMethod(function, args);
            return method.ReturnType;
        }

        /// <summary>
        ///     Gets method that fits name and types of arguments passed.
        /// </summary>
        /// <param name="name">Function name</param>
        /// <param name="methodArgs">Types of method arguments</param>
        /// <returns>Method that fits requirements.</returns>
        public MethodInfo GetMethod(string name, Type[] methodArgs)
        {
            var index = -1;
            if (!TryGetAnnotatedMethod(name, methodArgs, out index))
                throw new MethodNotFoundedException();

            return _methods[name][index];
        }

        /// <summary>
        /// Gets the registered method if exists.
        /// </summary>
        /// <param name="name">The method name.</param>
        /// <param name="methodArgs">The types of arguments methods contains.</param>
        /// <param name="result">Method metadas of founded method.</param>
        /// <returns>True if method exists, otherwise false.</returns>
        public bool TryGetMethod(string name, Type[] methodArgs, out MethodInfo result)
        {
            var index = -1;
            if (!TryGetAnnotatedMethod(name, methodArgs, out index))
            {
                result = null;
                return false;
            }

            result = _methods[name][index];
            return true;
        }

        /// <summary>
        ///     Determine if manager registered function with passed names and types of arguments.
        /// </summary>
        /// <param name="name">Function name</param>
        /// <param name="methodArgs">Types of method arguments</param>
        /// <returns>True if some method fits, else false.</returns>
        public bool HasMethod(string name, Type[] methodArgs)
        {
            int index;
            return TryGetAnnotatedMethod(name, methodArgs, out index) || TryGetRawMethod(name, methodArgs, out index);
        }

        /// <summary>
        ///     Tries match function as if it weren't annotated. Assume that method specified parameters explicitly.
        /// </summary>
        /// <param name="name">Function name</param>
        /// <param name="methodArgs">Types of method arguments</param>
        /// <param name="index">Index of method that fits requirements.</param>
        /// <returns>True if some method fits, else false.</returns>
        private bool TryGetRawMethod(string name, Type[] methodArgs, out int index)
        {
            if (!_methods.ContainsKey(name))
            {
                index = -1;
                return false;
            }

            var methods = _methods[name];

            for (var i = 0; i < methods.Count; ++i)
            {
                var method = methods[i];
                var parameters = method.GetParameters();

                if (parameters.Length != methodArgs.Length)
                    continue;

                var hasMatchedArgTypes = true;

                for (var j = 0; j < parameters.Length; ++j)
                {
                    if (parameters[j].ParameterType.GetUnderlyingNullable() == methodArgs[j])
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

        /// <summary>
        ///     Determine if there are registered functions with specific names and types of arguments.
        /// </summary>
        /// <param name="name">Function name</param>
        /// <param name="methodArgs">Types of method arguments</param>
        /// <param name="index">Index of method that fits requirements.</param>
        /// <returns>True if some method fits, else false.</returns>
        private bool TryGetAnnotatedMethod(string name, IReadOnlyList<Type> methodArgs, out int index)
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
                var optionalParametersCount = parameters.CountOptionalParameters();
                var allParameters = parameters.Length;
                var notAnnotatedParametersCount = parameters.CountWithoutParametersAnnotatedBy<InjectTypeAttribute>();
                var parametersToInject = allParameters - notAnnotatedParametersCount;

                //Wrong amount of argument's. That's not our function.
                if (HasMoreArgumentsThanMethodDefinitionContains(methodArgs, notAnnotatedParametersCount) ||
                    !CanUseSomeArgumentsAsDefaultParameters(methodArgs, notAnnotatedParametersCount,
                        optionalParametersCount))
                    continue;

                var parametersToSkip = parametersToInject;

                var hasMatchedArgTypes = true;
                for (int f = 0, g = methodArgs.Count; f < g; ++f)
                {
                    //1. When constant value, it won't be nullable<type> but type.
                    //So it is possible to call function with such value. 
                    //That's why GetUnderlyingNullable exists here.
                    if (IsTypePossibleToConvert(
                        parameters[f + parametersToSkip].ParameterType.GetUnderlyingNullable(),
                        methodArgs[f].GetUnderlyingNullable()))
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

        /// <summary>
        ///     Determine if there is more or equal values to pass to function that required parameters
        ///     and less or equal parameters than function definition has.
        /// </summary>
        /// <param name="methodArgs">Passed arguments to function.</param>
        /// <param name="parametersCount">All parameters count.</param>
        /// <param name="optionalParametersCount">Optional parameters count.</param>
        /// <returns></returns>
        private static bool CanUseSomeArgumentsAsDefaultParameters(IReadOnlyCollection<Type> methodArgs,
            int parametersCount, int optionalParametersCount)
        {
            return methodArgs.Count >= parametersCount - optionalParametersCount && methodArgs.Count <= parametersCount;
        }

        /// <summary>
        ///     Determine if passed arguments amount is greater than function can contain.
        /// </summary>
        /// <param name="methodArgs">Passed arguments.</param>
        /// <param name="parametersCount">Parameters amount.</param>
        /// <returns></returns>
        private static bool HasMoreArgumentsThanMethodDefinitionContains(IReadOnlyList<Type> methodArgs,
            int parametersCount) => methodArgs.Count > parametersCount;

        /// <summary>
        ///     Register new method.
        /// </summary>
        /// <param name="methodInfo">Method to register.</param>
        public void RegisterMethod(MethodInfo methodInfo) => RegisterMethod(methodInfo.Name, methodInfo);

        /// <summary>
        ///     Register new method.
        /// </summary>
        /// <param name="name">Name of method.</param>
        /// <param name="methodInfo">Method to register.</param>
        private void RegisterMethod(string name, MethodInfo methodInfo)
        {
            if (_methods.ContainsKey(name))
                _methods[name].Add(methodInfo);
            else
                _methods.Add(name, new List<MethodInfo> {methodInfo});
        }

        /// <summary>
        ///     Register methods of some name of some type
        /// </summary>
        /// <typeparam name="TType">Type where methods will be searched.</typeparam>
        /// <param name="methodName">Name of method to register.</param>
        public void RegisterMethods<TType>(string methodName)
        {
            var type = typeof(TType);
            var typeInfo = type.GetTypeInfo();
            var methods = typeInfo.GetDeclaredMethods(methodName);

            foreach (var m in methods)
                RegisterMethod(m.Name, m);
        }

        /// <summary>
        ///     Gets declaration list of registered methods identified by it's name.
        /// </summary>
        /// <param name="methodName">The method name.</param>
        /// <returns>Array of method declarations.</returns>
        private MethodDeclaration[] GetListOfUsedMethods(string methodName)
        {
            if (_methods.ContainsKey(methodName))
                return _methods[methodName].Select(f => new MethodDeclaration(f)).ToArray();
            throw new MethodNotFoundedException();
        }

        /// <summary>
        ///     Gets declaration list of all registered methods.
        /// </summary>
        /// <returns>Array of method declarations.</returns>
        public MethodDeclaration[] GetListOfUsedMethods()
        {
            var methods = new List<MethodDeclaration>();
            foreach (var m in _methods)
                methods.AddRange(GetListOfUsedMethods(m.Key));
            return methods.ToArray();
        }

        /// <summary>
        ///     Determine if type can be safely converted to another type.
        /// </summary>
        /// <param name="to">To what type will be converted.</param>
        /// <param name="from">From what type will be converted.</param>
        /// <returns>Return true if convertion is possible, otherwise false.</returns>
        public static bool IsTypePossibleToConvert(Type to, Type from)
        {
            if (TypeCompatibilityTable.ContainsKey(to))
                return TypeCompatibilityTable[to].Any(f => f == from);
            return false;
        }
    }
}