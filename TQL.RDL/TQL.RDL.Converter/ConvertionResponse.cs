using System.Collections.Generic;
using TQL.Core.Converters;
using TQL.RDL.Evaluator.ErrorHandling;

namespace TQL.RDL
{
    public class ConvertionResponse<T> : ConvertionResponseBase<T>
    {
        /// <summary>
        ///     Instantiate object of ConvertionResponse with proper properties instantied
        /// </summary>
        /// <param name="output">Instance of object that user wanted to obtain</param>
        /// <param name="messages">Message that potentially appeared after conversion</param>
        public ConvertionResponse(T output, params VisitationMessage[] messages)
        {
            Output = output;
            Messages = messages;
        }

        /// <summary>
        ///     Gets messages that could potentially appear after convertion from query to evaluator object
        /// </summary>
        public IReadOnlyCollection<VisitationMessage> Messages { get; }

        /// <summary>
        ///     Gets converted from query object (ie. evaluator)
        /// </summary>
        public T Output { get; }
    }
}