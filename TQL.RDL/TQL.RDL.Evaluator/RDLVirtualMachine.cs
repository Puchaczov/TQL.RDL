using System;
using System.Collections.Generic;
using System.Threading;
using TQL.Interfaces;
using TQL.RDL.Evaluator.Instructions;

namespace TQL.RDL.Evaluator
{
    public class RdlVirtualMachine : IFireTimeEvaluator, IVmTracker
    {
        private readonly bool _hasWhereConditions;
        private readonly CancellationToken _cancellationToken;
        private bool _isInBrokenState;

        /// <summary>
        /// Initialize instance.
        /// </summary>
        /// <param name="relativeLabels">Define labels and associated instructions.</param>
        /// <param name="instructions">Instruction set to execute.</param>
        /// <param name="stopAt">Stop time.</param>
        /// <param name="startAt">Start time.</param>
        /// <param name="hasWhereConditions">Determine if query has where condition.</param>
        public RdlVirtualMachine(Dictionary<string, int> relativeLabels, IRdlInstruction[] instructions,
            DateTimeOffset? stopAt, DateTimeOffset startAt, bool hasWhereConditions, CancellationToken token)
        {
            Values = new Stack<long>();
            Datetimes = new Stack<DateTimeOffset>();
            Strings = new Stack<string>();
            Variables = new MemoryVariables();
            Instructions = instructions;
            StopAt = stopAt;
            StartAt = startAt;
            ReferenceTime = startAt;
            RelativeLabels = relativeLabels;
            Registers = new long[2];
            _hasWhereConditions = hasWhereConditions;
            LastFireTime = null;
            _cancellationToken = token;
            _isInBrokenState = false;
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
            get { return Variables.ReferenceTime; }
            set { Variables.ReferenceTime = value; }
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

        /// <summary>
        /// Gets the latest fire time.
        /// </summary>
        public DateTimeOffset? LastFireTime { get; private set; }

        /// <summary>
        /// Determine if passed datetime fits the conditions.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>True if datetime fits the conditions, else false.</returns>
        public bool IsSatisfiedBy(DateTimeOffset date)
        {
            var oldReferenceTime = ReferenceTime;
            var oldLastFireTime = LastFireTime;
            ReferenceTime = date;
            LastFireTime = null;
            var processedDate = NextFire();
            ReferenceTime = oldReferenceTime;
            LastFireTime = oldLastFireTime;
            return processedDate.HasValue && processedDate.Value == date;
        }

        /// <summary>
        /// Perform evaluation of query based on passed instructions set.
        /// </summary>
        /// <returns>Next occurence or null if out of range.</returns>
        public DateTimeOffset? NextFire()
        {
            _cancellationToken.ThrowIfCancellationRequested();

            if (_isInBrokenState)
                return null;

            if (ReferenceTime < StartAt)
                ReferenceTime = StartAt;

            if (ReferenceTime > StopAt)
                Exit = true;

            try
            {

                while (true)
                {
                    _cancellationToken.ThrowIfCancellationRequested();

                    if (Exit)
                    {
                        LastFireTime = null;
                        return null;
                    }

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
                        Variables.Clear();

                        Exit = true;
                        LastFireTime = null;
                        return null;
                    }

                    var isCurrentDateFitsCondition = true;
                    if (_hasWhereConditions)
                        isCurrentDateFitsCondition = Convert.ToBoolean(Values.Pop());

                    Values.Clear();
                    Datetimes.Clear();
                    Variables.Clear();

                    if (isCurrentDateFitsCondition)
                    {
                        LastFireTime = old;
                        return old;
                    }
                }
            }
            catch(Exceptions.DivideByZeroException)
            {
                _isInBrokenState = true;
                throw;
            }
        }
    }
}