using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using TQL.RDL.Evaluator.Attributes;
using TQL.RDL.Parser.Helpers;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class CallExternal : IRdlInstruction
    {
        private readonly int _callParamsCount;
        private readonly MethodInfo _info;
        private readonly object _obj;

        /// <summary>
        ///     Initialize object.
        /// </summary>
        /// <param name="obj">object on that method will be executed</param>
        /// <param name="info">method to execute</param>
        /// <param name="callParamsCount">Count of parameters passed from AST</param>
        public CallExternal(object obj, MethodInfo info, int callParamsCount)
        {
            _obj = obj;
            _info = info;
            _callParamsCount = callParamsCount;
        }

        /// <summary>
        ///     Performs call on object.
        /// </summary>
        /// <param name="machine"></param>
        public void Run(RdlVirtualMachine machine)
        {
            var parameters = _info.GetParameters();
            var toInjectParams = parameters.GetParametersWithAttribute<InjectTypeAttribute>();

            var args = new List<object>();

            foreach (var parameterInfo in toInjectParams)
            {
                var attribute = parameterInfo.GetCustomAttribute<InjectTypeAttribute>();
                switch (attribute.GetType().Name)
                {
                    case nameof(InjectReferenceTimeAttribute):
                        args.Add(machine.ReferenceTime);
                        break;
                    case nameof(InjectLastFireAttribute):
                        args.Add(machine.LastFireTime);
                        break;
                }
            }

            var normalParams = parameters.GetParametersWithoutAttribute<InjectTypeAttribute>();

            for (var i = 0; i < normalParams.Length && i < _callParamsCount; i++)
            {
                var type = normalParams[i];

                switch (type.ParameterType.GetUnderlyingNullable().Name)
                {
                    case nameof(DateTimeOffset):
                        args.Add(machine.Datetimes.Pop());
                        break;
                    case nameof(String):
                        args.Add(machine.Strings.Pop());
                        break;
                    case nameof(Int64):
                        args.Add(machine.Values.Pop());
                        break;
                    case nameof(Int32):
                        args.Add(Convert.ToInt32(machine.Values.Pop()));
                        break;
                    case nameof(Int16):
                        args.Add(Convert.ToInt16(machine.Values.Pop()));
                        break;
                    case nameof(Boolean):
                        args.Add(Convert.ToBoolean(machine.Values.Pop()));
                        break;
                    default:
                        throw new Exception();
                }
            }

            var countOfUnsetParameters = parameters.Length - args.Count;

            if (countOfUnsetParameters > parameters.CountOptionalParameters())
                throw new ArgumentException();

            for (var i = 0; i < countOfUnsetParameters; ++i)
                args.Add(Type.Missing);

            var result = _info.Invoke(_obj, args.ToArray());

            switch (_info.ReturnType.GetUnderlyingNullable().Name)
            {
                case nameof(DateTimeOffset):
                    machine.Datetimes.Push((DateTimeOffset) result);
                    break;
                case nameof(String):
                    machine.Strings.Push((string) result);
                    break;
                case nameof(Boolean):
                case nameof(Int16):
                case nameof(Int32):
                case nameof(Int64):
                    machine.Values.Push(Convert.ToInt64(result));
                    break;
            }

            machine.InstructionPointer += 1;
        }

        /// <summary>
        ///     Gets instruction short description
        /// </summary>
        /// <returns>Stringified object.</returns>
        public override string ToString() => string.Format($"CALL {_info.Name}");
    }
}