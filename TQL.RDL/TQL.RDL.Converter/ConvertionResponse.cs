using System.Collections.Generic;
using TQL.Core.Converters;
using TQL.Interfaces;
using TQL.RDL.Evaluator;

namespace TQL.RDL.Converter
{
    public class ConvertionResponse<T> : ConvertionResponseBase<T>
    {
        public IReadOnlyCollection<VisitationMessage> Messages { get; }
        public T Output { get; }

        public ConvertionResponse(T output, params VisitationMessage[] messages)
        {
            this.Output = output;
            this.Messages = messages;
        }
    }
}
