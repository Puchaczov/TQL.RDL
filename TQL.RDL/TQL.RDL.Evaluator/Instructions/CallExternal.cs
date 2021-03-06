﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TQL.RDL.Evaluator.Attributes;
using TQL.RDL.Parser;
using TQL.RDL.Parser.Helpers;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class CallExternal : IRdlInstruction
    {
        private readonly int _callParamsCount;
        private readonly MethodInfo _info;
        private readonly ParameterInfo[] _infoParameters;
        private readonly ParameterInfo[] _withoutInjectParameters;
        private readonly InjectTypeAttribute[] _toInjectAttributes;
        private readonly object _obj;
        private readonly PartOfDate _partOfDate;
        private readonly int _functionOccurenceAmount;
        private readonly int _functionOrder;

        /// <summary>
        ///     Initialize object.
        /// </summary>
        /// <param name="obj">object on that method will be executed</param>
        /// <param name="info">method to execute</param>
        /// <param name="callParamsCount">Count of parameters passed from AST</param>
        /// <param name="partOfDate">Part of date</param>
        /// <param name="functionOccurenceAmount">Function occurence amount.</param>
        /// <param name="functionOrder">Function order</param>
        public CallExternal(object obj, MethodInfo info, int callParamsCount, PartOfDate partOfDate, int functionOccurenceAmount, int functionOrder)
        {
            _obj = obj;
            _info = info;
            _infoParameters = _info.GetParameters();
            _withoutInjectParameters = _infoParameters.GetParametersWithoutAttribute<InjectTypeAttribute>();
            var toInjectParameters = _infoParameters.GetParametersWithAttribute<InjectTypeAttribute>();
            _toInjectAttributes = toInjectParameters.Select(f => f.GetCustomAttribute<InjectTypeAttribute>()).ToArray();
            _callParamsCount = callParamsCount;
            _partOfDate = partOfDate;
            _functionOccurenceAmount = functionOccurenceAmount;
            _functionOrder = functionOrder;
        }

        /// <summary>
        ///     Performs call on object.
        /// </summary>
        /// <param name="machine"></param>
        public void Run(RdlVirtualMachine machine)
        {
            var parameters = _infoParameters;

            var args = new List<object>();

            foreach (var attribute in _toInjectAttributes)
            {
                switch (attribute.GetType().Name)
                {
                    case nameof(InjectReferenceTimeAttribute):
                        args.Add(machine.ReferenceTime);
                        break;
                    case nameof(InjectLastFireAttribute):
                        args.Add(machine.LastFireTime);
                        break;
                    case nameof(InjectPartOfDateTypeAttribute):
                        args.Add(_partOfDate);
                        break;
                    case nameof(InjectOccurenceOrderAttribute):
                        args.Add(_functionOrder);
                        break;
                    case nameof(InjectOccurencesAmountAttribute):
                        args.Add(_functionOccurenceAmount);
                        break;
                    case nameof(InjectStartAtAttribute):
                        args.Add(machine.StartAt);
                        break;
                    case nameof(InjectStopAtAttribute):
                        args.Add(machine.StopAt);
                        break;
                }
            }

            var normalParams = _withoutInjectParameters;

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

            for (int i = 0, j = parameters.Length - countOfUnsetParameters; i < countOfUnsetParameters; ++i)
            {
                var param = parameters[j + i];

                args.Add(param.HasDefaultValue ? param.DefaultValue : Type.Missing);
            }

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