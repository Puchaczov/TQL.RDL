using System;
using System.Reflection;
using TQL.Core.Converters;
using TQL.Interfaces;
using TQL.RDL.Evaluator;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Converter
{
    public class RdlTimeline : AbstractConverter<IFireTimeEvaluator>, IConverter<ConvertionRequest, ConvertionResponse<IFireTimeEvaluator>>
    {
        public RdlTimeline(bool throwOnError = false)
            : base(throwOnError)
        { }

        private ConvertionResponse<IFireTimeEvaluator> Convert(RootScriptNode ast, ConvertionRequest request)
        {
            MethodManager manager = new MethodManager();
            foreach(var pair in request.MethodsToBind)
            {
                manager.RegisterMethod(pair.Value.Name, pair.Key, pair.Value);
            }

            var visitor = new RDLCodeGenerationVisitor(manager);

            DefaultMethods methods = new DefaultMethods();
            manager.RegisterMethod(nameof(methods.IsDayOfWeek), methods, typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.IsDayOfWeek), new Type[] { typeof(DateTimeOffset), typeof(int) }));
            manager.RegisterMethod(nameof(methods.IsDayOfWeek), methods, typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.IsDayOfWeek), new Type[] { typeof(int) }));
            manager.RegisterMethod(nameof(methods.IsEven), methods, typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.IsEven), new Type[] { typeof(int) }));
            manager.RegisterMethod(nameof(methods.IsOdd), methods, typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.IsOdd), new Type[] { typeof(int) }));
            manager.RegisterMethod(nameof(methods.IsWorkingDay), methods, typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.IsWorkingDay), new Type[] { }));
            manager.RegisterMethod(nameof(methods.IsLastDayOfMonth), methods, typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.IsLastDayOfMonth), new Type[] { typeof(DateTimeOffset) }));
            manager.RegisterMethod(nameof(methods.IsLastDayOfMonth), methods, typeof(DefaultMethods).GetRuntimeMethod(nameof(methods.IsLastDayOfMonth), new Type[] { }));

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
    }
}
