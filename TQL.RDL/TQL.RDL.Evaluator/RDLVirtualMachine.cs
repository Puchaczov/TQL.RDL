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

    public class RDLVirtualMachine : IFireTimeEvaluator
    {
        private IRDLInstruction[] instructions;
        private int instrPtr;
        private DateTimeOffset? stopAt;
        private DateTimeOffset startAt;

        public MemoryVariables Variables { get; }
        private Func<DateTimeOffset?, DateTimeOffset?> GenerateNext;

        public Stack<long?> Values { get; }
        public Stack<DateTimeOffset?> Datetimes { get; }
        public object[] CallArgs { get; set; }

        public RDLVirtualMachine(Func<DateTimeOffset?, DateTimeOffset?> generateNext, IRDLInstruction[] instructions, DateTimeOffset? stopAt, DateTimeOffset startAt)
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
        }

        public DateTimeOffset? NextFire()
        {
            if(ReferenceTime < startAt)
            {
                ReferenceTime = startAt;
                return startAt;
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
    }
}
