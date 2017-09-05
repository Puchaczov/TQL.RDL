using System;
using System.Collections.Generic;
using System.Threading;
using TQL.Interfaces;
using TQL.RDL.Evaluator.ErrorHandling;

namespace TQL.RDL.Converter.Tests
{
    static class TestHelper
    {
        public static IFireTimeEvaluator ToEvaluator(string query) => Convert(query).Output;

        public static IReadOnlyCollection<VisitationMessage> ToMessages(string query) => Convert(query).Messages;

        public static ConvertionResponse<IFireTimeEvaluator> Convert(string query, CancellationToken token = default(CancellationToken))
            => Convert<DefaultMethodsAggregator>(query, token);

        public static ConvertionResponse<IFireTimeEvaluator> Convert<TMethodsAggregator>(string query)
            where TMethodsAggregator : new()
            => Convert<TMethodsAggregator>(query, TimeZoneInfo.Local);

        public static ConvertionResponse<IFireTimeEvaluator> Convert<TMethodsAggregator>(string query, CancellationToken cancellationToken)
            where TMethodsAggregator : new()
            => Convert<TMethodsAggregator>(query, TimeZoneInfo.Local, TimeZoneInfo.Local, cancellationToken);

        public static ConvertionResponse<IFireTimeEvaluator> Convert<TMethodsAggregator>(string query,
            TimeZoneInfo timezone)
            where TMethodsAggregator : new()
            => Convert<TMethodsAggregator>(query, TimeZoneInfo.Local, timezone);

        public static ConvertionResponse<IFireTimeEvaluator> Convert<TMethodsAggregator>(string query, TimeZoneInfo source, TimeZoneInfo destination, CancellationToken cancellationToken = default(CancellationToken))
            where TMethodsAggregator : new()
        {
            var request
                = new ConvertionRequest<TMethodsAggregator>(query, source, destination, false, new[]
                {
                    "dd/M/yyyy H:m:s",
                    "dd/M/yyyy h:m:s tt",
                    "dd.M.yyyy H:m:s",
                    "dd.M.yyyy h:m:s tt",
                    "yyyy-mm.dd HH:mm:ss",
                    "yyyy/mm/dd H:m:s",
                    "dd.M.yyyy"
                }, cancellationToken);

            var timeline = new RdlTimeline<TMethodsAggregator>(false);

            var response = timeline.Convert(request);

            return response;
        }
    }
}