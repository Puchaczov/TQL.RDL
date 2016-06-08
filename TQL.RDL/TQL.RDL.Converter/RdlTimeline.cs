using System;
using System.Reflection;
using TQL.Common.Converters;
using TQL.Interfaces;
using TQL.RDL.Evaluator;
using TQL.RDL.Parser;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Converter
{
    public class RdlTimeline : AbstractConverter<IFireTimeEvaluator>, IConvertible<ConvertionRequest, ConvertionResponse<IFireTimeEvaluator>>
    {
        public RdlTimeline(bool throwOnError = false)
            : base(throwOnError)
        { }

        private ConvertionResponse<IFireTimeEvaluator> Convert(RootScriptNode ast, ConvertionRequest request)
        {
            foreach(var method in request.MethodsToBind)
            {
                GlobalMetadata.RegisterMethod(method.Name, method);
            }

            var visitor = new RDLCodeGenerationVisitor();

            DefaultMethods methods = new DefaultMethods();

            GlobalMetadata.RegisterMethod(nameof(methods.IsDayOfWeek), typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.IsDayOfWeek), new Type[] { typeof(DateTimeOffset?), typeof(long?) }));
            GlobalMetadata.RegisterMethod(nameof(methods.IsDayOfWeek), typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.IsDayOfWeek), new Type[] { typeof(long?) }));

            GlobalMetadata.RegisterMethod(nameof(methods.IsEven), typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.IsEven), new Type[] { typeof(long?) }));
            GlobalMetadata.RegisterMethod(nameof(methods.IsOdd), typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.IsOdd), new Type[] { typeof(long?) }));
            GlobalMetadata.RegisterMethod(nameof(methods.IsWorkingDay), typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.IsWorkingDay), new Type[] { }));

            GlobalMetadata.RegisterMethod(nameof(methods.IsLastDayOfMonth), typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.IsLastDayOfMonth), new Type[] { typeof(DateTimeOffset?) }));
            GlobalMetadata.RegisterMethod(nameof(methods.IsLastDayOfMonth), typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.IsLastDayOfMonth), new Type[] { }));

            GlobalMetadata.RegisterMethod(nameof(methods.GetDate), typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.GetDate), new Type[] { }));

            GlobalMetadata.RegisterMethod(nameof(methods.GetYear), typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.GetYear), new Type[] { }));
            GlobalMetadata.RegisterMethod(nameof(methods.GetMonth), typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.GetMonth), new Type[] { }));
            GlobalMetadata.RegisterMethod(nameof(methods.GetDay), typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.GetDate), new Type[] { }));

            GlobalMetadata.RegisterMethod(nameof(methods.GetHour), typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.GetHour), new Type[] { }));
            GlobalMetadata.RegisterMethod(nameof(methods.GetMinute), typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.GetMinute), new Type[] { }));
            GlobalMetadata.RegisterMethod(nameof(methods.GetSecond), typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.GetSecond), new Type[] { }));

            GlobalMetadata.RegisterMethod(nameof(methods.Now), typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.Now), new Type[] { }));
            GlobalMetadata.RegisterMethod(nameof(methods.UtcNow), typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.UtcNow), new Type[] { }));

            ast.Accept(visitor);
            
            var evaluator = visitor.VirtualMachine;

            methods.SetMachine(evaluator);

            if (evaluator != null)
            {
                if(!evaluator.ReferenceTime.HasValue)
                    evaluator.ReferenceTime = request.ReferenceTime;
                return new ConvertionResponse<IFireTimeEvaluator>(evaluator);
            }
            return new ConvertionResponse<IFireTimeEvaluator>(null);
        }

        public ConvertionResponse<IFireTimeEvaluator> Convert(ConvertionRequest request) => base.Convert(request, (ast) => this.Convert(ast, request));

        protected override ConvertionResponse<IFireTimeEvaluator> GetErrorResponse(Exception exc)
        {
            throw exc;
        }
    }
}
