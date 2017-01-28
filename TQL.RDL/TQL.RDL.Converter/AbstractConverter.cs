using RDL.Parser;
using RDL.Parser.Nodes;
using RDL.Parser.Tokens;
using TQL.Core.Converters;
using TQL.RDL.Evaluator;

namespace TQL.RDL.Converter
{
    public abstract class AbstractConverter<TOutput, TMethodsAggregator> : ConverterBase<TOutput, ConvertionResponse<TOutput>, INodeVisitor, StatementType, RootScriptNode, ConvertionRequest<TMethodsAggregator>>
        where TMethodsAggregator : new()
    {
        /// <summary>
        /// Default date formats acceptable by DateTimeOffset.Parse(...) object.
        /// </summary>
        private readonly string[] _defaultFormats = {
            "dd/M/yyyy H:m:s",
            "dd/M/yyyy h:m:s tt",
            "dd.M.yyyy H:m:s",
            "dd.M.yyyy h:m:s tt"
        };

        #region Private variables

        private bool _throwOnError;

        #endregion

        /// <summary>
        /// Construct object with information if error should be thrown when convertion failed.
        /// </summary>
        /// <param name="throwOnError">Determine if errors should be aggregated or rethrown.</param>
        protected AbstractConverter(bool throwOnError)
            : base(throwOnError)
        {
            _throwOnError = throwOnError;
        }

        /// <summary>
        /// Gets metadata cache object
        /// </summary>
        protected abstract RdlMetadata Metdatas { get; }

        /// <summary>
        /// Parse requested query into Abstract Syntax Tree
        /// </summary>
        /// <param name="request">User defined request</param>
        /// <returns>Abstract syntax tree</returns>
        protected override RootScriptNode InstantiateRootNodeFromRequest(ConvertionRequest<TMethodsAggregator> request)
        {
            var preprocessor = new Preprocessor.Preprocessor();
            var query = preprocessor.Execute(request.Query);
            var lexer = new LexerComplexTokensDecorator(query);
            RdlParser parser = null;

            if (request.Formats == null || request.Formats.Length == 0)
                parser = new RdlParser(lexer, request.Source.BaseUtcOffset, _defaultFormats, request.CultureInfo, new MethodDeclarationResolver(Metdatas));
            else
                parser = new RdlParser(lexer, request.Source.BaseUtcOffset, request.Formats, request.CultureInfo, new MethodDeclarationResolver(Metdatas));

            return parser.ComposeRootComponents();
        }

        /// <summary>
        /// Determine if request is valid (it doesn't validate if query is valid)
        /// </summary>
        /// <param name="request">User defined request</param>
        /// <returns>Return if request is valid</returns>
        protected override bool IsValid(ConvertionRequest<TMethodsAggregator> request) => !string.IsNullOrEmpty(request.Query);
    }
}
