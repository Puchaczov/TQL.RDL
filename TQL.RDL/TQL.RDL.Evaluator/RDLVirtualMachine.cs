using System;
using System.Collections.Generic;
using TQL.Interfaces;

namespace TQL.RDL.Evaluator
{
    public class MemoryVariables : Dictionary<string, object>
    {
        public DateTimeOffset? Current => (DateTimeOffset?)base["current"];
        public int? Second => Current?.Second;
        public int? Minute => Current?.Minute;
        public int? Hour => Current?.Hour;
        public int? Day => Current?.Day;
        public int? Month => Current?.Month;
        public int? Year => Current?.Year;
    }

    public enum Registers : short
    {
        A,
        B
    }

    public class StackFrame
    {
        public Stack<long?> Values;
        public Stack<DateTimeOffset?> Datetimes;
        public object[] CallArgs { get; set; }
        public long[] regs { get; set; }
        public IRDLInstruction[] instructions;
        public StackFrame()
        {

        }
    }

    public interface IVmTracker
    {
        DateTimeOffset StartAt { get; }
        DateTimeOffset? StopAt { get; }
        IRDLInstruction[] Instructions { get; }
        Stack<long?> Values { get; }
        Stack<DateTimeOffset?> Datetimes { get; }
        object[] CallArgs { get; }
    }

    public class RDLVirtualMachine : IFireTimeEvaluator, IVmTracker
    {
        private IRDLInstruction[] instructions;
        private int instrPtr;
        private DateTimeOffset? stopAt;
        private DateTimeOffset startAt;
        private long[] regs = new long[2];

        public Dictionary<string, int> RelativeLabels { get; }

        public MemoryVariables Variables { get; }
        private Func<DateTimeOffset?, DateTimeOffset?> GenerateNext;

        public Stack<long?> Values { get; }
        public Stack<DateTimeOffset?> Datetimes { get; }
        public object[] CallArgs { get; set; }

        public RDLVirtualMachine(Dictionary<string, int> relativeLabels, Func<DateTimeOffset?, DateTimeOffset?> generateNext, IRDLInstruction[] instructions, DateTimeOffset? stopAt, DateTimeOffset startAt)
        {
            Values = new Stack<long?>();
            Datetimes = new Stack<DateTimeOffset?>();
            GenerateNext = generateNext;
            Variables = new MemoryVariables();
            Variables["current"] = DateTimeOffset.Now;
            this.instructions = instructions;
            this.stopAt = stopAt;
            this.startAt = startAt;
            ReferenceTime = startAt;
            RelativeLabels = relativeLabels;
            regs = new long[2];
            LastlyFound = null;
        }

        public DateTimeOffset? NextFire()
        {
            if(ReferenceTime < startAt)
            {
                ReferenceTime = startAt;
            }

            if(ReferenceTime > stopAt)
            {
                ReferenceTime = null;
                return null;
            }

            while(true)
            {
                if (Exit)
                    return null;

                if (!ReferenceTime.HasValue)
                    return null;

                Break = false;

                instrPtr = 0;

                IRDLInstruction instruction = null;

                var old = ReferenceTime;

                Datetimes.Push(ReferenceTime);

                while (!Break && !Exit)
                {
                    instruction = instructions[instrPtr];
                    instruction.Run(this);
                }

                ReferenceTime = Datetimes.Pop();

                if (stopAt.HasValue && old > stopAt.Value)
                {
                    ReferenceTime = null;
                    return null;
                }

                bool cond = false;
                if (Values.Count > 0)
                    cond = Convert.ToBoolean(Values.Pop());
                else
                    cond = true;

                if (cond)
                    return old;
            }
        }

        public DateTimeOffset? ReferenceTime
        {
            get
            {
                return (DateTimeOffset?)Variables["current"];
            }
            set
            {
                Variables["current"] = value;
            }
        }

        public DateTimeOffset? LastlyFound
        {
            get
            {
                return (DateTimeOffset?)Variables["lastlyFound"];
            }
            set
            {
                Variables["lastlyFound"] = value;
            }
        }

        public bool Break { get; set; }

        public bool Exit { get; set; }

        public int InstructionPointer
        {
            get
            {
                return instrPtr;
            }
            set
            {
                instrPtr = value;
            }
        }

        public long[] Registers => regs;

        public DateTimeOffset StartAt => startAt;

        public DateTimeOffset? StopAt => stopAt;

        public IRDLInstruction[] Instructions => instructions;
    }
}
