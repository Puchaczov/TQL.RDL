using TQL.RDL.Parser.Nodes;
using TQL.Core.Converters;
using TQL.RDL.Parser.Tokens;
using TQL.RDL.Parser;
using TQL.RDL.Evaluator;

namespace TQL.RDL.Converter
{
    public abstract class AbstractConverter<TOutput> : ConverterBase<TOutput, ConvertionResponse<TOutput>, INodeVisitor, StatementType, RootScriptNode, ConvertionRequest>
    {
        #region Private variables

        private bool _throwOnError;

        #endregion

        /// <summary>
        /// Gets metadata cache object
        /// </summary>
        protected abstract RdlMetadata Metdatas { get; }

        /// <summary>
        /// Construct object with information if error should be thrown when convertion failed.
        /// </summary>
        /// <param name="throwOnError">Determine if errors should be aggregated or rethrown.</param>
        protected AbstractConverter(bool throwOnError)
            : base(throwOnError)
        {
            _throwOnError = throwOnError;

            RegisterDefaultMethods();
        }

        /// <summary>
        /// Default date formats acceptable by DateTimeOffset.Parse(...) object.
        /// </summary>
        private readonly string[] _defaultFormats = {
            "dd/M/yyyy H:m:s",
            "dd/M/yyyy h:m:s tt",
            "dd.M.yyyy H:m:s",
            "dd.M.yyyy h:m:s tt"
        };

        /// <summary>
        /// Parse requested query into Abstract Syntax Tree
        /// </summary>
        /// <param name="request">User defined request</param>
        /// <returns>Abstract syntax tree</returns>
        protected override RootScriptNode InstantiateRootNodeFromRequest(ConvertionRequest request)
        {
            var preprocessor = new Preprocessor.Preprocessor();
            var query = preprocessor.Execute(request.Query);
            var lexer = new LexerComplexTokensDecorator(query);
            RdlParser parser = null;

            if (request.Formats == null || request.Formats.Length == 0)
                parser = new RdlParser(lexer, Metdatas, request.Source.BaseUtcOffset, _defaultFormats, request.CultureInfo);
            else
                parser = new RdlParser(lexer, Metdatas, request.Source.BaseUtcOffset, request.Formats, request.CultureInfo);

            return parser.ComposeRootComponents();
        }

        /// <summary>
        /// Determine if request is valid (it doesn't validate if query is valid)
        /// </summary>
        /// <param name="request">User defined request</param>
        /// <returns>Return if request is valid</returns>
        protected override bool IsValid(ConvertionRequest request) => !string.IsNullOrEmpty(request.Query);

        /// <summary>
        /// Register methods which can be used by user in his query.
        /// </summary>
        private void RegisterDefaultMethods()
        {
            Metdatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.IsDayOfWeek));
            Metdatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.IsEven));
            Metdatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.IsOdd));
            Metdatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.IsWorkingDay));
            Metdatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.IsLastDayOfMonth));
            Metdatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetDate));
            Metdatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetYear));
            Metdatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetMonth));
            Metdatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetDay));
            Metdatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetHour));
            Metdatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetMinute));
            Metdatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetSecond));
            Metdatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetWeekOfMonth));
            Metdatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.Now));
            Metdatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.UtcNow));
            Metdatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetDayOfWeek));
            Metdatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetDayOfYear));
        }
    }
}
