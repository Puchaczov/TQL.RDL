﻿using System;
using System.Collections.Generic;
using TQL.Interfaces;
using TQL.RDL.Evaluator.Instructions;

namespace TQL.RDL.Evaluator
{
    public class RdlVirtualMachine : IFireTimeEvaluator, IVmTracker
    {
        public RdlVirtualMachine(Dictionary<string, int> relativeLabels, IRdlInstruction[] instructions, DateTimeOffset? stopAt, DateTimeOffset startAt)
        {
            Values = new Stack<long>();
            Datetimes = new Stack<DateTimeOffset>();
            Strings = new Stack<string>();
            Variables = new MemoryVariables {["current"] = DateTimeOffset.Now};
            Instructions = instructions;
            StopAt = stopAt;
            StartAt = startAt;
            ReferenceTime = startAt;
            RelativeLabels = relativeLabels;
            Registers = new long[2];
            LastlyFound = null;
        }

        public Dictionary<string, int> RelativeLabels { get; }

        public MemoryVariables Variables { get; }

        public DateTimeOffset ReferenceTime
        {
            get
            {
                return (DateTimeOffset)Variables["current"];
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

        private bool Exit { get; set; }

        public int InstructionPointer { get; set; }

        public long[] Registers { get; }

        public DateTimeOffset? NextFire()
        {
            if(ReferenceTime < StartAt)
            {
                ReferenceTime = StartAt;
            }

            if(ReferenceTime > StopAt)
            {
                Exit = true;
            }

            while(true)
            {
                if (Exit)
                    return null;

                Break = false;

                InstructionPointer = 0;

                var old = ReferenceTime;

                while (!Break && !Exit)
                {
                    var instruction = Instructions[InstructionPointer];
                    instruction.Run(this);
                }

                if (StopAt.HasValue && old > StopAt.Value)
                {
                    Values.Clear();
                    Datetimes.Clear();

                    Exit = true;
                    return null;
                }

                var cond = false;

                if (Values.Count > 0)
                    cond = Convert.ToBoolean(Values.Pop());
                else
                    cond = true;

                Values.Clear();
                Datetimes.Clear();

                if (cond)
                    return old;
            }
        }

        public Stack<long> Values { get; }
        public Stack<DateTimeOffset> Datetimes { get; }

        public DateTimeOffset StartAt { get; }

        public DateTimeOffset? StopAt { get; }

        public IRdlInstruction[] Instructions { get; }

        public Stack<string> Strings { get; }
    }
}
