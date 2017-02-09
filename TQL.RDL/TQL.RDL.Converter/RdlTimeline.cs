using System;
using System.Linq;
using System.Reflection;
using RDL.Parser.Helpers;
using RDL.Parser.Nodes;
using TQL.Common.Converters;
using TQL.Common.Evaluators;
using TQL.Interfaces;
using TQL.RDL.Evaluator.Attributes;
using TQL.RDL.Evaluator.Visitors;
using TQL.RDL.Converter.Exceptions;
using TQL.RDL.Evaluator;

namespace TQL.RDL.Converter
{
    public class RdlTimeline<TMethodsAggregator> : AbstractConverter<IFireTimeEvaluator, TMethodsAggregator>, IConvertible<ConvertionRequest<TMethodsAggregator>, ConvertionResponse<IFireTimeEvaluator>> 
        where TMethodsAggregator : new()
    {
        /// <summary>
        /// Instantiate to evaluator converter.
        /// </summary>
        /// <param name="throwOnError">Allow errors to be aggregated or rethrowed immediatelly</param>
        public RdlTimeline(bool throwOnError = false)
            : base(throwOnError)
        {
            RegisterQueryMethods();
        }

        /// <summary>
        /// It's just metadata cache.
        /// </summary>
        protected override RdlMetadata Metdatas { get; } = new RdlMetadata();

        /// <summary>
        /// Convert user request into reponse that contains evaluator
        /// </summary>
        /// <param name="request">Query convertion request</param>
        /// <returns>Response with evaluator</returns>
        public ConvertionResponse<IFireTimeEvaluator> Convert(ConvertionRequest<TMethodsAggregator> request) => base.Convert(request, ast => Convert(ast, request));

        /// <summary>
        /// Convert Abstract Syntax Tree to Virtual Machine object with associated virtual code for such machine
        /// </summary>
        /// <param name="ast">Abstract Syntax Tree.</param>
        /// <param name="request">Request used by user to perform convertion.</param>
        /// <returns>Object that allows evaluate next occurences</returns>
        private ConvertionResponse<IFireTimeEvaluator> Convert(RootScriptNode ast, ConvertionRequest<TMethodsAggregator> request)
        {
            var coretnessChecker = new RdlQueryValidator(Metdatas);
            var queryValidatorTraverser = new Traverser(coretnessChecker);
            ast.Accept(queryValidatorTraverser);

            if (!coretnessChecker.IsValid)
                return new ConvertionResponse<IFireTimeEvaluator>(null, coretnessChecker.Errors.ToArray());

            var codeGenerator = request.Debuggable 
                ? new DebuggerSymbolGenerator(Metdatas, request.MethodsAggregator) : 
                new CodeGenerator(Metdatas, request.MethodsAggregator);

            var codeGenerationTraverseVisitor = new ExtendedTraverser(codeGenerator, MethodOccurences);

            ast.Accept(codeGenerationTraverseVisitor);
            var evaluator = codeGenerator.VirtualMachine;

            if (evaluator == null)
                return new ConvertionResponse<IFireTimeEvaluator>(null, coretnessChecker.Errors.ToArray());
            return request.Source == request.Target ? new ConvertionResponse<IFireTimeEvaluator>(evaluator) : new ConvertionResponse<IFireTimeEvaluator>(new TimeZoneChangerDecorator(request.Target, evaluator));
        }

        /// <summary>
        /// Register methods of TMethodsAggregator that are properly annotated.
        /// </summary>
        private void RegisterQueryMethods()
        {
            var type = typeof(TMethodsAggregator);
            var typeInfo = type.GetTypeInfo();

            if (typeInfo.GetCustomAttribute<BindableClassAttribute>() != null)
            {
                var methods = type
                    .GetRuntimeMethods().Where(f => f.IsDefined(typeof(BindableMethodAttribute), false));

                foreach (var method in methods)
                {
                    CheckMethodHasInjectableOptionalOrDefaultParameters(method);
                    Metdatas.RegisterMethod(method);
                }
            }
        }

        /// <summary>
        /// Checks if method fit injection conditions.
        /// </summary>
        /// <param name="method">method that will be analysed.</param>
        private void CheckMethodHasInjectableOptionalOrDefaultParameters(MethodInfo method)
        {
            var parameters = method.GetParameters();
            var injectedParams = parameters.GetParametersWithAttribute<InjectTypeAttribute>();

            if (injectedParams.Any(f => f.IsOptional))
                throw new InjectParameterCannotBeOptionalException(method.Name);

            if (injectedParams.Any(f => f.HasDefaultValue))
                throw new InjectParameterHasDefaultValueException(method.Name);
        }

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
