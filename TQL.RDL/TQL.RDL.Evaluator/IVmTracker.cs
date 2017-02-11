using System;
using System.Collections.Generic;
using TQL.RDL.Evaluator.Instructions;

namespace TQL.RDL.Evaluator
{
    public interface IVmTracker
    {
        /// <summary>
        /// Gets a query start at value.
        /// </summary>
        DateTimeOffset StartAt { get; }

        /// <summary>
        /// Gets a query stop at value.
        /// </summary>
        DateTimeOffset? StopAt { get; }

        /// <summary>
        /// Gets query instruction set.
        /// </summary>
        IRdlInstruction[] Instructions { get; }

        /// <summary>
        /// Values stack.
        /// </summary>
        Stack<long> Values { get; }

        /// <summary>
        /// Datetime stack.
        /// </summary>
        Stack<DateTimeOffset> Datetimes { get; }

        /// <summary>
        /// Strings stack.
        /// </summary>
        Stack<string> Strings { get; }
    }
}