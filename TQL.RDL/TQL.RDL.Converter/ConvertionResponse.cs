using TQL.Core.Converters;
using TQL.Interfaces;
using TQL.RDL.Evaluator;

namespace TQL.RDL.Converter
{
    public class ConvertionResponse<T> : ConvertionResponseBase<T>
    {
        public T Output { get; }

        public ConvertionResponse(T output)
        {
            this.Output = output;
        }
    }
}
