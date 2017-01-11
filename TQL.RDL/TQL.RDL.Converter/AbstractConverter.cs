using TQL.RDL.Parser.Nodes;
using TQL.Core.Converters;
using TQL.RDL.Parser.Tokens;
using TQL.RDL.Parser;
using TQL.RDL.Evaluator;

namespace TQL.RDL.Converter
{
    public abstract class AbstractConverter<TOutput> : ConverterBase<TOutput, ConvertionResponse<TOutput>, INodeVisitor, StatementType, RootScriptNode, ConvertionRequest>
    {
        private bool throwOnError;
        protected readonly RdlMetadata metadatas;

        protected AbstractConverter(bool throwOnError, RdlMetadata metadatas)
            : base(throwOnError)
        {
            this.throwOnError = throwOnError;
            this.metadatas = metadatas;

            RegisterDefaultMethods();
        }

        private readonly string[] defaultFormats = new string[] {
            "dd/M/yyyy H:m:s",
            "dd/M/yyyy h:m:s tt",
            "dd.M.yyyy H:m:s",
            "dd.M.yyyy h:m:s tt"
        };

        protected override RootScriptNode InstantiateRootNodeFromRequest(ConvertionRequest request)
        {
            var preprocessor = new Preprocessor.Preprocessor();
            var query = preprocessor.Execute(request.Query);
            var lexer = new LexerComplexTokensDecorator(query);
            RdlParser parser = null;

            if (request.Formats == null || request.Formats.Length == 0)
                parser = new RdlParser(lexer, metadatas, request.Source.BaseUtcOffset, defaultFormats, request.CultureInfo);
            else
                parser = new RdlParser(lexer, metadatas, request.Source.BaseUtcOffset, request.Formats, request.CultureInfo);

            return parser.ComposeRootComponents();
        }

        protected override bool IsValid(ConvertionRequest request) => !string.IsNullOrEmpty(request.Query);

        private void RegisterDefaultMethods()
        {
            metadatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.IsDayOfWeek));
            metadatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.IsEven));
            metadatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.IsOdd));
            metadatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.IsWorkingDay));
            metadatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.IsLastDayOfMonth));
            metadatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetDate));
            metadatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetYear));
            metadatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetMonth));
            metadatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetDay));
            metadatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetHour));
            metadatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetMinute));
            metadatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetSecond));
            metadatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetWeekOfMonth));
            metadatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.Now));
            metadatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.UtcNow));
            metadatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetDayOfWeek));
            metadatas.RegisterMethods<DefaultMethods>(nameof(DefaultMethods.GetDayOfYear));
        }
    }
}
