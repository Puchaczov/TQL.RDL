using System.Diagnostics;
using RDL.Parser;

namespace TQL.RDL.Evaluator.Instructions
{
    [DebuggerDisplay("{GetType().Name,nq}: {ToString(),nq}")]
    public class AddNumericToDatetime : IRdlInstruction
    {
        private readonly PartOfDate _partOfDate;
        private readonly int _value;

        /// <summary>
        /// Initialize object.
        /// </summary>
        /// <param name="partOfDate">Part of date that will be modified.</param>
        /// <param name="value">Value that will be added to datetime.</param>
        public AddNumericToDatetime(PartOfDate partOfDate, int value)
        {
            _partOfDate = partOfDate;
            _value = value;
        }

        /// <summary>
        /// Initialize object
        /// </summary>
        /// <param name="machine"></param>
        public void Run(RdlVirtualMachine machine)
        {
            switch (_partOfDate)
            {
                case PartOfDate.Seconds:
                    machine.Datetimes.Push(machine.Datetimes.Pop().AddSeconds(_value));
                    break;
                case PartOfDate.Minutes:
                    machine.Datetimes.Push(machine.Datetimes.Pop().AddMinutes(_value));
                    break;
                case PartOfDate.Hours:
                    machine.Datetimes.Push(machine.Datetimes.Pop().AddHours(_value));
                    break;
                case PartOfDate.DaysOfMonth:
                    machine.Datetimes.Push(machine.Datetimes.Pop().AddDays(_value));
                    break;
                case PartOfDate.Months:
                    machine.Datetimes.Push(machine.Datetimes.Pop().AddMonths(_value));
                    break;
                case PartOfDate.Years:
                    machine.Datetimes.Push(machine.Datetimes.Pop().AddYears(_value));
                    break;
            }
            machine.InstructionPointer += 1;
        }

        /// <summary>
        /// Gets instruction short description
        /// </summary>
        /// <returns>Stringified object.</returns>
        public override string ToString() => "ADD";
    }
}