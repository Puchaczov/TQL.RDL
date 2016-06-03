﻿using System;
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

        public MemoryVariables Variables { get; }
        private Func<DateTimeOffset?, DateTimeOffset?> GenerateNext;

        public Stack<long?> Values { get; }
        public Stack<DateTimeOffset?> Datetimes { get; }
        public object[] CallArgs { get; set; }

        public RDLVirtualMachine(Func<DateTimeOffset?, DateTimeOffset?> generateNext, IRDLInstruction[] instructions, DateTimeOffset? stopAt)
        {
            Values = new Stack<long?>();
            Datetimes = new Stack<DateTimeOffset?>();
            GenerateNext = generateNext;
            Variables = new MemoryVariables();
            Variables["current"] = DateTimeOffset.Now;
            this.instructions = instructions;
            this.stopAt = stopAt;
        }

        public DateTimeOffset? NextFire()
        {
            while(true)
            {
                if (Exit)
                    return null;

                Break = false;

                instrPtr = 0;

                IRDLInstruction instruction = null;
                bool isRightTime = true;

                Datetimes.Push(ReferenceTime);

                while (!Break && !Exit)
                {
                    instruction = instructions[instrPtr];
                    instruction.Run(this);
                }
                
                isRightTime = Values.Count == 0 || (Values.Count > 0 && Convert.ToBoolean(Values.Pop().Value));

                if (isRightTime && (!stopAt.HasValue || stopAt.Value >= Datetimes.Peek()))
                {
                    var stored = Datetimes.Pop();
                    var old = ReferenceTime;
                    ReferenceTime = stored;
                    return old;
                }

                if (stopAt.HasValue && Datetimes.Peek() > stopAt.Value)
                    return null;

                if (!ReferenceTime.HasValue)
                    return null;

                ReferenceTime = Datetimes.Pop();
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
