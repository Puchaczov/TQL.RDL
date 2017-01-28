using System;
using System.Collections.Generic;
using TQL.RDL.Evaluator.Instructions;

namespace TQL.RDL.Evaluator
{
    public interface IVmTracker
    {
        DateTimeOffset StartAt { get; }
        DateTimeOffset? StopAt { get; }
        IRdlInstruction[] Instructions { get; }
        Stack<long> Values { get; }
        Stack<DateTimeOffset> Datetimes { get; }
        Stack<string> Strings { get; }
    }
}