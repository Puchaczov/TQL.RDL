using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RDL.Parser.Helpers;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class PrepareFunctionCall : IRdlInstruction
    {
        private readonly IEnumerable<Type> _enumerable;
        private readonly int _parametersLength;
        private readonly int _optionalParameters;

        public PrepareFunctionCall(IEnumerable<Type> enumerable, int parametersLength, int optionalParameters)
        {
            _enumerable = enumerable;
            _parametersLength = parametersLength;
            _optionalParameters = optionalParameters;
        }

        public void Run(RdlVirtualMachine machine)
        {
            var args = _enumerable.Select(f => {
                switch(f.GetTypeName())
                {
                    case nameof(DateTimeOffset):
                        return (object)machine.Datetimes.Pop();
                    case nameof(String):
                        return (object) machine.Strings.Pop();
                    case nameof(Int64):
                    case nameof(Boolean):
                        return (object)machine.Values.Pop();
                    default:
                        throw new Exception();
                }
            }).ToList();

            var countOfUnsetParameters = (_parametersLength - args.Count);

            if(countOfUnsetParameters > _optionalParameters)
                throw new ArgumentException();

            for (var i = 0; i < countOfUnsetParameters; ++i)
            {
                args.Add(Type.Missing);
            }

            machine.CallArgs = args.ToArray();
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "PREPARE FUNCTION CALL";
    }
}