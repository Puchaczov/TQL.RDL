using TQL.RDL.Parser.Nodes;
using TQL.Core.Converters;
using TQL.RDL.Parser.Tokens;
using TQL.RDL.Parser;

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
    }
}
