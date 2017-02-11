using System;
using System.Collections.Generic;
using TQL.Interfaces;
using TQL.RDL.Evaluator.Instructions;

namespace TQL.RDL.Evaluator
{
    public class RdlVirtualMachine : IFireTimeEvaluator, IVmTracker
    {
        /// <summary>
        /// Initialize instance.
        /// </summary>
        /// <param name="relativeLabels">Define labels and associated instructions.</param>
        /// <param name="instructions">Instruction set to execute.</param>
        /// <param name="stopAt">Stop time.</param>
        /// <param name="startAt">Start time.</param>
        public RdlVirtualMachine(Dictionary<string, int> relativeLabels, IRdlInstruction[] instructions,
            DateTimeOffset? stopAt, DateTimeOffset startAt)
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

        /// <summary>
        /// Get labels and associated instructions.
        /// </summary>
        public Dictionary<string, int> RelativeLabels { get; }

        /// <summary>
        /// Gets storage of variables.
        /// </summary>
        public MemoryVariables Variables { get; }

        /// <summary>
        /// Gets or sets ReferenceTime
        /// </summary>
        public DateTimeOffset ReferenceTime
        {
            get { return (DateTimeOffset) Variables["current"]; }
            set { Variables["current"] = value; }
        }

        /// <summary>
        /// Gets of sets Lastly found
        /// </summary>
        public DateTimeOffset? LastlyFound
        {
            get { return (DateTimeOffset?) Variables["lastlyFound"]; }
            set { Variables["lastlyFound"] = value; }
        }

        /// <summary>
        /// Gets or sets value to force break evaluator.
        /// </summary>
        public bool Break { get; set; }

        /// <summary>
        /// Gets or sets value to force evaluator to exit.
        /// </summary>
        private bool Exit { get; set; }

        /// <summary>
        /// Gets or sets InstructionPointer.
        /// </summary>
        public int InstructionPointer { get; set; }

        /// <summary>
        /// Gets registers of evaluator.
        /// </summary>
        public long[] Registers { get; }

        /// <summary>
        /// Perform evaluation of query based on passed instructions set.
        /// </summary>
        /// <returns></returns>
        public DateTimeOffset? NextFire()
        {
            if (ReferenceTime < StartAt)
                ReferenceTime = StartAt;

            if (ReferenceTime > StopAt)
                Exit = true;

            while (true)
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

        /// <summary>
        /// Gets values stack.
        /// </summary>
        public Stack<long> Values { get; }

        /// <summary>
        /// Gets dates stack.
        /// </summary>
        public Stack<DateTimeOffset> Datetimes { get; }

        /// <summary>
        /// Gets StartAt that determine where evaluation starts from.
        /// </summary>
        public DateTimeOffset StartAt { get; }

        /// <summary>
        /// Gets StopAt that determine where evaluation will be stopped.
        /// </summary>
        public DateTimeOffset? StopAt { get; }

        /// <summary>
        /// The instructions set.
        /// </summary>
        public IRdlInstruction[] Instructions { get; }

        /// <summary>
        /// Gets string stack.
        /// </summary>
        public Stack<string> Strings { get; }
    }
}