using System;
using System.Linq;
using TQL.Common.Converters;
using TQL.Common.Evaluators;
using TQL.Interfaces;
using TQL.RDL.Evaluator;
using TQL.RDL.Evaluator.Visitors;
using TQL.RDL.Parser;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Converter
{
    public class RdlTimeline : AbstractConverter<IFireTimeEvaluator>, IConvertible<ConvertionRequest, ConvertionResponse<IFireTimeEvaluator>>
    {
        public RdlTimeline(bool throwOnError = false)
            : base(throwOnError, new RdlMetadata())
        { }

        private ConvertionResponse<IFireTimeEvaluator> Convert(RootScriptNode ast, ConvertionRequest request)
        {
            foreach(var method in request.MethodsToBind)
            {
                metadatas.RegisterMethod(method.Name, method);
            }


            var coretnessChecker = new RDLQueryValidator(metadatas);
            var queryValidatorTraverser = new CodeGenerationTraverser(coretnessChecker);
            ast.Accept(queryValidatorTraverser);

            if (!coretnessChecker.IsValid)
                return new ConvertionResponse<IFireTimeEvaluator>(null, coretnessChecker.Errors.ToArray());

            var codeGenerator = request.Debuggable ? new RDLDebuggerSymbolGenerator(metadatas) : new RDLCodeGenerator(metadatas);
            var codeGenerationTraverseVisitor = new CodeGenerationTraverser(codeGenerator);

            ast.Accept(codeGenerationTraverseVisitor);
            var evaluator = codeGenerator.VirtualMachine;

            if (evaluator == null)
                return new ConvertionResponse<IFireTimeEvaluator>(null, coretnessChecker.Errors.ToArray());
            return request.Source == request.Target ? new ConvertionResponse<IFireTimeEvaluator>(evaluator) : new ConvertionResponse<IFireTimeEvaluator>(new TimeZoneChangerDecorator(request.Target, evaluator));
        }

        public ConvertionResponse<IFireTimeEvaluator> Convert(ConvertionRequest request) => base.Convert(request, (ast) => this.Convert(ast, request));

        protected override ConvertionResponse<IFireTimeEvaluator> GetErrorResponse(Exception exc)
        {
            throw exc;
        }
    }
}
