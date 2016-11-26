using TQL.RDL.Parser.Nodes;
using TQL.Core.Converters;
using TQL.RDL.Parser.Tokens;
using TQL.RDL.Parser;
using TQL.RDL.Evaluator;
using System.Reflection;
using System;

namespace TQL.RDL.Converter
{
    public abstract class AbstractConverter<TOutput> : ConverterBase<TOutput, ConvertionResponse<TOutput>, INodeVisitor, StatementType, RootScriptNode, ConvertionRequest>
    {
        protected bool throwOnError;
        protected RdlMetadata metadatas;

        protected AbstractConverter(bool throwOnError, RdlMetadata metadatas)
            : base(throwOnError)
        {
            this.throwOnError = throwOnError;
            this.metadatas = metadatas;

            this.RegisterDefaultMethods();
        }

        protected override RootScriptNode InstantiateRootNodeFromRequest(ConvertionRequest request)
        {
            var preprocessor = new Preprocessor();
            var query = preprocessor.Execute(request.Query);
            var lexer = new LexerComplexTokensDecorator(query);
            var parser = new RDLParser(lexer, metadatas);
            return parser.ComposeRootComponents();
        }

        protected override bool IsValid(ConvertionRequest request) => true;

        private void RegisterDefaultMethods()
        {
            metadatas.RegisterMethod(nameof(DefaultMethods.IsDayOfWeek), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.IsDayOfWeek), new Type[] { typeof(DateTimeOffset?), typeof(long?) }));
            metadatas.RegisterMethod(nameof(DefaultMethods.IsDayOfWeek), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.IsDayOfWeek), new Type[] { typeof(long?) }));

            metadatas.RegisterMethod(nameof(DefaultMethods.IsEven), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.IsEven), new Type[] { typeof(long?) }));
            metadatas.RegisterMethod(nameof(DefaultMethods.IsOdd), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.IsOdd), new Type[] { typeof(long?) }));
            metadatas.RegisterMethod(nameof(DefaultMethods.IsWorkingDay), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.IsWorkingDay), new Type[] { }));

            metadatas.RegisterMethod(nameof(DefaultMethods.IsLastDayOfMonth), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.IsLastDayOfMonth), new Type[] { typeof(DateTimeOffset?) }));
            metadatas.RegisterMethod(nameof(DefaultMethods.IsLastDayOfMonth), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.IsLastDayOfMonth), new Type[] { }));

            metadatas.RegisterMethod(nameof(DefaultMethods.GetDate), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.GetDate), new Type[] { }));

            metadatas.RegisterMethod(nameof(DefaultMethods.GetYear), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.GetYear), new Type[] { }));
            metadatas.RegisterMethod(nameof(DefaultMethods.GetMonth), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.GetMonth), new Type[] { }));
            metadatas.RegisterMethod(nameof(DefaultMethods.GetDay), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.GetDate), new Type[] { }));

            metadatas.RegisterMethod(nameof(DefaultMethods.GetHour), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.GetHour), new Type[] { }));
            metadatas.RegisterMethod(nameof(DefaultMethods.GetMinute), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.GetMinute), new Type[] { }));
            metadatas.RegisterMethod(nameof(DefaultMethods.GetSecond), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.GetSecond), new Type[] { }));

            metadatas.RegisterMethod(nameof(DefaultMethods.Now), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.Now), new Type[] { }));
            metadatas.RegisterMethod(nameof(DefaultMethods.UtcNow), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.UtcNow), new Type[] { }));
        }
    }
}
