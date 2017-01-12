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
        /// <summary>
        /// Instantiate to evaluator converter.
        /// </summary>
        /// <param name="throwOnError">Allow errors to be aggregated or rethrowed immediatelly</param>
        public RdlTimeline(bool throwOnError = false)
            : base(throwOnError)
        { }

        /// <summary>
        /// It's just metadata cache.
        /// </summary>
        protected override RdlMetadata Metdatas { get; } = new RdlMetadata();

        /// <summary>
        /// Convert Abstract Syntax Tree to Virtual Machine object with associated virtual code for such machine
        /// </summary>
        /// <param name="ast">Abstract Syntax Tree.</param>
        /// <param name="request">Request used by user to perform convertion.</param>
        /// <returns>Object that allows evaluate next occurences</returns>
        private ConvertionResponse<IFireTimeEvaluator> Convert(RootScriptNode ast, ConvertionRequest request)
        {
            foreach(var method in request.MethodsToBind)
            {
                Metdatas.RegisterMethod(method.Name, method);
            }

            var coretnessChecker = new RdlQueryValidator(Metdatas);
            var queryValidatorTraverser = new CodeGenerationTraverser(coretnessChecker);
            ast.Accept(queryValidatorTraverser);

            if (!coretnessChecker.IsValid)
                return new ConvertionResponse<IFireTimeEvaluator>(null, coretnessChecker.Errors.ToArray());

            var codeGenerator = request.Debuggable ? new RdlDebuggerSymbolGenerator(Metdatas) : new RdlCodeGenerator(Metdatas);
            var codeGenerationTraverseVisitor = new CodeGenerationTraverser(codeGenerator);

            ast.Accept(codeGenerationTraverseVisitor);
            var evaluator = codeGenerator.VirtualMachine;

            if (evaluator == null)
                return new ConvertionResponse<IFireTimeEvaluator>(null, coretnessChecker.Errors.ToArray());
            return request.Source == request.Target ? new ConvertionResponse<IFireTimeEvaluator>(evaluator) : new ConvertionResponse<IFireTimeEvaluator>(new TimeZoneChangerDecorator(request.Target, evaluator));
        }

        /// <summary>
        /// Convert user request into reponse that contains evaluator
        /// </summary>
        /// <param name="request">Query convertion request</param>
        /// <returns>Response with evaluator</returns>
        public ConvertionResponse<IFireTimeEvaluator> Convert(ConvertionRequest request) => base.Convert(request, (ast) => Convert(ast, request));

        /// <summary>
        /// Construct convertion reposne if fatal error occured
        /// </summary>
        /// <param name="exc">Fatal exception that had been thrown.</param>
        /// <returns>Convertion response with that fail.</returns>
        protected override ConvertionResponse<IFireTimeEvaluator> GetErrorResponse(Exception exc)
        {
            throw exc;
        }
    }
}
