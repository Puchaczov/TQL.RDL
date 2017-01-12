using System;
using System.Collections.Generic;
using TQL.Interfaces;

namespace TQL.RDL.Evaluator
{
    public class RdlVirtualMachine : IFireTimeEvaluator, IVmTracker
    {
        private readonly IRdlInstruction[] _instructions;
        private int _instrPtr;
        private DateTimeOffset? _stopAt;
        private DateTimeOffset _startAt;
        private readonly long[] _regs;

        public Dictionary<string, int> RelativeLabels { get; }

        public MemoryVariables Variables { get; }
        private Func<DateTimeOffset, DateTimeOffset> _generateNext;

        public Stack<long> Values { get; }
        public Stack<DateTimeOffset> Datetimes { get; }
        public object[] CallArgs { get; set; }

        public RdlVirtualMachine(Dictionary<string, int> relativeLabels, Func<DateTimeOffset, DateTimeOffset> generateNext, IRdlInstruction[] instructions, DateTimeOffset? stopAt, DateTimeOffset startAt)
        {
            Values = new Stack<long>();
            Datetimes = new Stack<DateTimeOffset>();
            _generateNext = generateNext;
            Variables = new MemoryVariables {["current"] = DateTimeOffset.Now};
            _instructions = instructions;
            _stopAt = stopAt;
            _startAt = startAt;
            ReferenceTime = startAt;
            RelativeLabels = relativeLabels;
            _regs = new long[2];
            LastlyFound = null;
        }

        public DateTimeOffset? NextFire()
        {
            if(ReferenceTime < _startAt)
            {
                ReferenceTime = _startAt;
            }

            if(ReferenceTime > _stopAt)
            {
                Exit = true;
            }

            while(true)
            {
                if (Exit)
                    return null;

                Break = false;

                _instrPtr = 0;

                IRdlInstruction instruction = null;

                var old = ReferenceTime;

                Datetimes.Push(ReferenceTime);

                while (!Break && !Exit)
                {
                    instruction = _instructions[_instrPtr];
                    instruction.Run(this);
                }

                ReferenceTime = Datetimes.Pop();

                if (_stopAt.HasValue && old > _stopAt.Value)
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

        public int InstructionPointer
        {
            get
            {
                return _instrPtr;
            }
            set
            {
                _instrPtr = value;
            }
        }

        public long[] Registers => _regs;

        public DateTimeOffset StartAt => _startAt;

        public DateTimeOffset? StopAt => _stopAt;

        public IRdlInstruction[] Instructions => _instructions;
    }
}
