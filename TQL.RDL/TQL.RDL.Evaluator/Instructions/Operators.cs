using RDL.Parser.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace TQL.RDL.Evaluator.Instructions
{

    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class GoToInstruction : IRDLInstruction
    {
        private int newInstructionPtr;

        public GoToInstruction(int newInstructionPtr)
        {
            this.newInstructionPtr = newInstructionPtr;
        }

        public void Run(RDLVirtualMachine machine)
        {
            machine.InstructionPointer = newInstructionPtr;
        }

        public override string ToString() => newInstructionPtr.ToString();
    }


    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class BreakInstruction : IRDLInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Break = true;
            machine.InstructionPointer += 1;
        }
    }


    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class CallExternalNumeric : IRDLInstruction
    {
        protected MethodInfo info;
        protected object obj;

        public CallExternalNumeric(object obj, MethodInfo info)
        {
            this.obj = obj;
            this.info = info;
        }

        public void Run(RDLVirtualMachine machine)
        {
            var result = info.Invoke(obj, machine.CallArgs);
            machine.Values.Push(Convert.ToBoolean(result) ? 1 : 0);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => string.Format("CALL");
    }

    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class CallExternalDatetime : IRDLInstruction
    {
        protected MethodInfo info;
        protected object obj;

        public CallExternalDatetime(object obj, MethodInfo info)
        {
            this.obj = obj;
            this.info = info;
        }

        public void Run(RDLVirtualMachine machine)
        {
            var result = info.Invoke(obj, machine.CallArgs);
            machine.Datetimes.Push((DateTimeOffset?)result);
            machine.InstructionPointer += 1;
        }
    }


    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class AndInstruction : IRDLInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push(machine.Values.Pop() & machine.Values.Pop());
            machine.InstructionPointer += 1;
        }
    }


    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class OrInstruction : IRDLInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push(machine.Values.Pop() | machine.Values.Pop());
            machine.InstructionPointer += 1;
        }
    }

    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class PushDateTimeInstruction : IRDLInstruction
    {
        private DateTimeOffset value;

        public PushDateTimeInstruction(DateTimeOffset value)
        {
            this.value = value;
        }

        public void Run(RDLVirtualMachine machine)
        {
            machine.Datetimes.Push(value);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => string.Format("PUSH {0}", value);
    }


    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class PushNumericInstruction : IRDLInstruction
    {
        private long value;

        public PushNumericInstruction(long value)
        {
            this.value = value;
        }

        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push(value);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => string.Format("PUSH {0}", value);
    }


    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class LoadNumericVariableInstruction : IRDLInstruction
    {
        private Func<MemoryVariables, int?> loadFun;

        public LoadNumericVariableInstruction(Func<MemoryVariables, int?> func)
        {
            this.loadFun = func;
        }

        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push(loadFun(machine.Variables));
            machine.InstructionPointer += 1;
        }

    }


    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class LoadDateTimeVariableInstruction : IRDLInstruction
    {
        private Func<MemoryVariables, DateTimeOffset?> loadFun;

        public LoadDateTimeVariableInstruction(Func<MemoryVariables, DateTimeOffset?> func)
        {
            this.loadFun = func;
        }

        public void Run(RDLVirtualMachine machine)
        {
            machine.Datetimes.Push(loadFun(machine.Variables));
            machine.InstructionPointer += 1;
        }
    }


    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class EqualityNumeric : IRDLInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push((machine.Values.Pop() == machine.Values.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }
    }


    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class EqualityDatetime : IRDLInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push((machine.Datetimes.Pop() == machine.Datetimes.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }
    }


    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class DiffNumeric : IRDLInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push((machine.Values.Pop() != machine.Values.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }
    }


    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class DiffDatetime : IRDLInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push((machine.Datetimes.Pop() != machine.Datetimes.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }
    }

    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class GreaterEqualNumeric : IRDLInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push((machine.Values.Pop() >= machine.Values.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }
    }

    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class GreaterEqualDatetime : IRDLInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push((machine.Datetimes.Pop() >= machine.Datetimes.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }
    }

    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class GreaterNumeric : IRDLInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push((machine.Values.Pop() > machine.Values.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }
    }

    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class GreaterDatetime : IRDLInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push((machine.Datetimes.Pop() > machine.Datetimes.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }
    }

    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class LessEqualNumeric : IRDLInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push((machine.Values.Pop() <= machine.Values.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "MODIFY";
    }

    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class LessEqualDatetime : IRDLInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push((machine.Datetimes.Pop() <= machine.Datetimes.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "LESSEQUAL-DATETIME";
    }

    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class LessNumeric : IRDLInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push((machine.Values.Pop() < machine.Values.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "LESS-NUMERIC";
    }

    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class LessDatetime : IRDLInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push((machine.Datetimes.Pop() < machine.Datetimes.Pop()) ? 1 : 0);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "LESS-DATETIME";
    }


    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public abstract class InInstruction<T> : IRDLInstruction
        where T : struct
    {
        protected delegate Nullable<T> PopFun(RDLVirtualMachine machine);
        protected delegate void PushFun(RDLVirtualMachine machine, Nullable<T> value);
        protected delegate Nullable<T> PeekFun(RDLVirtualMachine machine);

        private PopFun pop;
        private PushFun push;
        private PeekFun peek;

        protected InInstruction(PopFun popFun, PushFun pushFun, PeekFun peekFun)
        {
            pop = popFun;
            push = pushFun;
            peek = peekFun;
        }

        public void Run(RDLVirtualMachine machine)
        {
            var inArgsCount = machine.Values.Pop();

            if (!inArgsCount.HasValue)
                throw new ArgumentOutOfRangeException(nameof(inArgsCount));

            var toCompare = pop(machine);

            if (!toCompare.HasValue)
                push(machine, default(T));
            else
            {

                var result = false;
                int i = 0;
                for (; i < inArgsCount && !result; ++i)
                {
                    var tmpRes = pop(machine);
                    result |= tmpRes.HasValue && tmpRes.Value.Equals(toCompare.Value);
                }
                for (; i < inArgsCount; ++i)
                {
                    pop(machine);
                }
                machine.Values.Push(result ? 1 : 0);
            }
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "IN-GENERIC";
    }

    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class InInstructionNumeric : InInstruction<long>
    {
        public InInstructionNumeric()
            : base((x) => x.Values.Pop(), (x, v) => x.Values.Push(v), (x) => x.Values.Peek())
        { }
        public override string ToString() => "IN-NUMERIC";
    }

    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class InInstructionDatetime : InInstruction<DateTimeOffset>
    {
        public InInstructionDatetime()
            : base((x) => x.Datetimes.Pop(), (x, v) => x.Datetimes.Push(v), (x) => x.Datetimes.Peek())
        { }

        public override string ToString() => "IN-DATETIME";
    }

    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class Modify : IRDLInstruction
    {
        public delegate void Fun(RDLVirtualMachine machine);

        private Fun fun;

        public Modify(Fun fun)
        {
            this.fun = fun;
        }

        public void Run(RDLVirtualMachine machine)
        {
            fun(machine);
            machine.InstructionPointer += 1;
        }

        public override string ToString() => "MODIFY";
    }

    public class NotInstruction : IRDLInstruction
    {
        public void Run(RDLVirtualMachine machine)
        {
            machine.Values.Push(~machine.Values.Pop().Value);
        }
    }

    public class PrepareFunctionCall : IRDLInstruction
    {
        private IEnumerable<Type> enumerable;

        public PrepareFunctionCall(IEnumerable<Type> enumerable)
        {
            this.enumerable = enumerable;
        }

        public void Run(RDLVirtualMachine machine)
        {
            object[] args = enumerable.Select(f => {
                switch(f.GetTypeName())
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
    }
}
