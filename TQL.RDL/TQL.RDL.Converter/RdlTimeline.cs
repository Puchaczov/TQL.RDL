using System;
using System.Linq;
using System.Reflection;
using TQL.Common.Converters;
using TQL.Interfaces;
using TQL.RDL.Evaluator;
using TQL.RDL.Parser;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Converter
{
    public class RdlTimeline : AbstractConverter<IFireTimeEvaluator>, IConvertible<ConvertionRequest, ConvertionResponse<IFireTimeEvaluator>>
    {
        public RdlTimeline(bool throwOnError = false)
            : base(throwOnError)
        { }

        private ConvertionResponse<IFireTimeEvaluator> Convert(RootScriptNode ast, ConvertionRequest request)
        {
            foreach(var method in request.MethodsToBind)
            {
                GlobalMetadata.RegisterMethod(method.Name, method);
            }

            RDLQueryValidator coretnessChecker = new RDLQueryValidator();
            ast.Accept(coretnessChecker);

            if(coretnessChecker.IsValid)
            {
                RDLCodeGenerator codeGenerator = null;
                if (request.Debuggable)
                {
                    codeGenerator = new RDLDebuggerSymbolGeneratorVisitor();
                }
                else
                {
                    codeGenerator = new RDLCodeGenerator();
                }

                ast.Accept(codeGenerator);
                var evaluator = codeGenerator.VirtualMachine;

                if (evaluator != null)
                {
                    if (!evaluator.ReferenceTime.HasValue)
                        evaluator.ReferenceTime = request.ReferenceTime;
                    return new ConvertionResponse<IFireTimeEvaluator>(evaluator);
                }
            }
            return new ConvertionResponse<IFireTimeEvaluator>(null, coretnessChecker.Errors.ToArray());
        }

        public ConvertionResponse<IFireTimeEvaluator> Convert(ConvertionRequest request) => base.Convert(request, (ast) => this.Convert(ast, request));

        protected override ConvertionResponse<IFireTimeEvaluator> GetErrorResponse(Exception exc)
        {
            throw exc;
        }
    }
}
