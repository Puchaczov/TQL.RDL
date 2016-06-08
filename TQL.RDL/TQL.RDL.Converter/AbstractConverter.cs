using TQL.RDL.Parser.Nodes;
using TQL.Core.Converters;
using TQL.RDL.Parser.Tokens;
using TQL.RDL.Parser;
using TQL.RDL.Evaluator;
using System.Reflection;
using System;

namespace TQL.RDL.Converter
{
    public abstract class AbstractConverter<TOutput> : ConverterBase<TOutput, ConvertionResponse<TOutput>, INodeVisitor, SyntaxType, RootScriptNode, ConvertionRequest>
    {
        protected bool throwOnError;

        protected AbstractConverter(bool throwOnError)
            : base(throwOnError)
        {
            this.throwOnError = throwOnError;
        }

        protected override RootScriptNode InstantiateRootNodeFromRequest(ConvertionRequest request)
        {
            var lexer = new LexerComplexTokensDecorator(request.Query);
            var parser = new RDLParser(lexer);
            return parser.ComposeRootComponents();
        }

        protected override bool IsValid(ConvertionRequest request) => true;

        static AbstractConverter()
        {
            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.IsDayOfWeek), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.IsDayOfWeek), new Type[] { typeof(DateTimeOffset?), typeof(long?) }));
            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.IsDayOfWeek), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.IsDayOfWeek), new Type[] { typeof(long?) }));

            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.IsEven), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.IsEven), new Type[] { typeof(long?) }));
            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.IsOdd), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.IsOdd), new Type[] { typeof(long?) }));
            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.IsWorkingDay), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.IsWorkingDay), new Type[] { }));

            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.IsLastDayOfMonth), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.IsLastDayOfMonth), new Type[] { typeof(DateTimeOffset?) }));
            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.IsLastDayOfMonth), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.IsLastDayOfMonth), new Type[] { }));

            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.GetDate), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.GetDate), new Type[] { }));

            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.GetYear), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.GetYear), new Type[] { }));
            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.GetMonth), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.GetMonth), new Type[] { }));
            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.GetDay), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.GetDate), new Type[] { }));

            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.GetHour), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.GetHour), new Type[] { }));
            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.GetMinute), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.GetMinute), new Type[] { }));
            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.GetSecond), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.GetSecond), new Type[] { }));

            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.Now), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.Now), new Type[] { }));
            GlobalMetadata.RegisterMethod(nameof(DefaultMethods.UtcNow), typeof(DefaultMethods).GetRuntimeMethod(nameof(DefaultMethods.UtcNow), new Type[] { }));
        }
    }
}
