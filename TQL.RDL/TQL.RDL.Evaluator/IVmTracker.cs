using System;
using System.Collections.Generic;

namespace TQL.RDL.Evaluator
{
    public interface IVmTracker
    {
        DateTimeOffset StartAt { get; }
        DateTimeOffset? StopAt { get; }
        IRdlInstruction[] Instructions { get; }
        Stack<long> Values { get; }
        Stack<DateTimeOffset> Datetimes { get; }
        object[] CallArgs { get; }
    }
}