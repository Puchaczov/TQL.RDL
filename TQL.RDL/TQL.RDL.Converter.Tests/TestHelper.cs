using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TQL.Interfaces;
using TQL.RDL.Evaluator.ErrorHandling;

namespace TQL.RDL.Converter.Tests
{
    static class TestHelper
    {
        public static IFireTimeEvaluator ToEvaluator(string query) => Convert(query).Output;

        public static IReadOnlyCollection<VisitationMessage> ToMessages(string query) => Convert(query).Messages;

        public static ConvertionResponse<IFireTimeEvaluator> Convert(string query)
            => Convert<DefaultMethodsAggregator>(query);

        public static ConvertionResponse<IFireTimeEvaluator> Convert<TMethodsAggregator>(string query)
            where TMethodsAggregator : new()
        {
            ConvertionRequest<TMethodsAggregator> request
                = new ConvertionRequest<TMethodsAggregator>(query, TimeZoneInfo.Local, TimeZoneInfo.Local, false, new[] {
                    "dd/M/yyyy H:m:s",
                    "dd/M/yyyy h:m:s tt",
                    "dd.M.yyyy H:m:s",
                    "dd.M.yyyy h:m:s tt",
                    "yyyy-mm.dd HH:mm:ss",
                    "yyyy/mm/dd H:m:s",
                    "dd.M.yyyy"
            });

            RdlTimeline<TMethodsAggregator> timeline = new RdlTimeline<TMethodsAggregator>(false);

            var response = timeline.Convert(request);

            return response;
        }
    }
}
