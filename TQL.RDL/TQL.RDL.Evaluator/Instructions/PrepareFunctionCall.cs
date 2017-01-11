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
        private IEnumerable<Type> enumerable;

        public PrepareFunctionCall(IEnumerable<Type> enumerable)
        {
            this.enumerable = enumerable;
        }

        public void Run(RDLVirtualMachine machine)
        {
            object[] args = enumerable.Select(f => {
                switch(TypeHelper.GetTypeName((Type) f))
                {
                    case nameof(DateTimeOffset):
                        return (object)machine.Datetimes.Pop();
                    case nameof(Int64):
                    case nameof(Boolean):
                        return (object)machine.Values.Pop();
                    default:
                        throw new Exception();
                }
            }).ToArray();
            machine.CallArgs = args;
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "PREPARE FUNCTION CALL";
    }
}