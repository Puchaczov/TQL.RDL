﻿using System;
using System.Linq;
using System.Reflection;
using TQL.Common.Converters;
using TQL.Common.Evaluators;
using TQL.Interfaces;
using TQL.RDL.Evaluator;
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

            RDLQueryValidator coretnessChecker = new RDLQueryValidator(metadatas);
            ast.Accept(coretnessChecker);

            if(coretnessChecker.IsValid)
            {
                RDLCodeGenerator codeGenerator = null;
                if (request.Debuggable)
                {
                    codeGenerator = new RDLDebuggerSymbolGeneratorVisitor(metadatas, request.MinDate, request.MaxDate);
                }
                else
                {
                    codeGenerator = new RDLCodeGenerator(metadatas, request.MinDate, request.MaxDate);
                }

                ast.Accept(codeGenerator);
                var evaluator = codeGenerator.VirtualMachine;

                if (evaluator != null)
                {
                    if (!evaluator.ReferenceTime.HasValue)
                        evaluator.ReferenceTime = request.ReferenceTime;

                    return new ConvertionResponse<IFireTimeEvaluator>(new TimeZoneChangerDecorator(request.Target, evaluator));
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
