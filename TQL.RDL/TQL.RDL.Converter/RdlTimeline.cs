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
            ast.Accept(visitor);
            var evaluator = visitor.VirtualMachine;
            if(evaluator != null)
            {
                evaluator.ReferenceTime = request.ReferenceTime;
                return new ConvertionResponse<IFireTimeEvaluator>(evaluator);
            }
            return new ConvertionResponse<IFireTimeEvaluator>(null);
        }

        public ConvertionResponse<IFireTimeEvaluator> Convert(ConvertionRequest request) => base.Convert(request, (ast) => this.Convert(ast, request));
    }
}
