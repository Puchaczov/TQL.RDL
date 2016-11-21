using System;
using System.Collections.Generic;
using System.Linq;
using TQL.Core.Tokens;
using TQL.RDL.Parser;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator
{
    public class RDLQueryValidator : RDLAnalyzerBase
    {
        protected readonly List<Exception> criticalErrors;
        private readonly List<VisitationMessage> errors;

        public virtual IEnumerable<VisitationMessage> Errors => e;
        protected IReadOnlyList<VisitationMessage> e => errors.Concat(criticalErrors.Select(f => new FatalVisitError(f))).ToArray();

        public virtual bool IsValid => criticalErrors.Count == 0 && errors.Count == 0;

        public RDLQueryValidator()
        {
            this.criticalErrors = new List<Exception>();
            this.errors = new List<VisitationMessage>();
        }

        public override void Visit(WhereConditionsNode node)
        { }

        public override void Visit(RepeatEveryNode node)
        {
            try
            {
                if (node.DatePart == RepeatEveryNode.PartOfDate.Unknown)
                {
                    ReportUnknownRepeatEveryDatePartFraction(node);
                }
            }
            catch(Exception e)
            {
                criticalErrors.Add(e);
            }
        }

        public override void Visit(OrNode node)
        { }

        public override void Visit(DateTimeNode node)
        { }

        public override void Visit(EqualityNode node)
        { }

        public override void Visit(ArgListNode node)
        { }

        public override void Visit(NumericNode node)
        { }

        public override void Visit(GreaterEqualNode node)
        { }

        public override void Visit(LessEqualNode node)
        { }

        public override void Visit(AddNode node)
        { }

        public override void Visit(ModuloNode node)
        { }

        public override void Visit(FSlashNode node)
        { }

        public override void Visit(ThenNode node)
        { }

        public override void Visit(CaseNode node)
        { }

        public override void Visit(WhenThenNode node)
        { }

        public override void Visit(ElseNode node)
        { }

        public override void Visit(WhenNode node)
        { }

        public override void Visit(StarNode node)
        { }

        public override void Visit(HyphenNode node)
        { }

        public override void Visit(NumericConsequentRepeatEveryNode node)
        { }

        public override void Visit(LessNode node)
        { }

        public override void Visit(GreaterNode node)
        { }

        public override void Visit(VarNode node)
        { }

        public override void Visit(NotInNode node)
        { }

        public override void Visit(DiffNode node)
        { }

        public override void Visit(InNode node)
        {
            try
            {
                var dstType = node.Left.ReturnType;

                bool hasMixedTypes = false;
                foreach (var desc in node.Right.Descendants)
                {
                    if (dstType != desc.ReturnType)
                    {
                        hasMixedTypes = true;
                        break;
                    }
                }

                if (hasMixedTypes)
                {
                    ReportHasMixedTypes(node);
                }
            }
            catch (Exception e)
            {
                criticalErrors.Add(e);
            }
        }

        public override void Visit(AndNode node)
        { }

        public override void Visit(StartAtNode node)
        { }

        public override void Visit(StopAtNode node)
        { }

        public override void Visit(WordNode node)
        { }

        public override void Visit(FunctionNode node)
        {
            try
            {
                if (!GlobalMetadata.HasMethod(node.Name, node.Args.Descendants.Select(f => f.ReturnType).ToArray()))
                {
                    ReportUnknownFunctionCall(node);
                }
            }
            catch (Exception e)
            {
                criticalErrors.Add(e);
            }
        }

        private void ReportHasMixedTypes(InNode node)
        {
            this.AddSemanticError(node.FullSpan, AnalysisMessage.MixedTypesNotAllowed, SemanticErrorKind.MixedValues);
        }

        private void ReportUnknownFunctionCall(FunctionNode node)
        {
            this.AddSyntaxError(node.FullSpan, string.Format(AnalysisMessage.UnknownFunctionCall, node.Name, node.Args.Descendants.Select(f => f.ReturnType.Name).ToArray()), SyntaxErrorKind.UnsupportedFunctionCall);
        }

        private void ReportUnknownRepeatEveryDatePartFraction(RepeatEveryNode node)
        {
            this.AddSyntaxError(node.FullSpan, string.Format(AnalysisMessage.RepeateEveryContainsUnknownDatePartFraction, node.Token.Value), SyntaxErrorKind.WrongKeyword);
        }

        private void AddSemanticError(TextSpan span, string message, SemanticErrorKind kind)
        {
            AddSemanticError(new TextSpan[] { span }, message, kind);
        }

        private void AddSemanticError(TextSpan[] spans, string message, SemanticErrorKind kind)
        {
            errors.Add(new SemanticError(spans, message, kind));
        }

        private void AddSyntaxError(TextSpan fullSpan, string v, SyntaxErrorKind missingValue)
        {
            errors.Add(new SyntaxError(fullSpan, v, missingValue));
        }
    }
}
