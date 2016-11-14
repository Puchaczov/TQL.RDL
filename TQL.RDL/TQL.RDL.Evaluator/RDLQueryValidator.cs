using System;
using System.Collections.Generic;
using System.Linq;
using TQL.Core.Tokens;
using TQL.RDL.Parser;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator
{
    public class RDLQueryValidator : INodeVisitor
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

        public void Visit(WhereConditionsNode node)
        { }

        public void Visit(RepeatEveryNode node)
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

        public void Visit(OrNode node)
        { }

        public void Visit(DateTimeNode node)
        { }

        public void Visit(EqualityNode node)
        { }

        public void Visit(ArgListNode node)
        { }

        public void Visit(NumericNode node)
        { }

        public void Visit(GreaterEqualNode node)
        { }

        public void Visit(LessEqualNode node)
        { }

        public void Visit(AddNode node)
        { }

        public void Visit(ModuloNode node)
        { }

        public void Visit(FSlashNode node)
        { }

        public void Visit(ThenNode node)
        { }

        public void Visit(CaseNode node)
        { }

        public void Visit(WhenThenNode node)
        { }

        public void Visit(ElseNode node)
        { }

        public void Visit(WhenNode node)
        { }

        public void Visit(StarNode node)
        { }

        public void Visit(HyphenNode node)
        { }

        public void Visit(NumericConsequentRepeatEveryNode node)
        { }

        public void Visit(LessNode node)
        { }

        public void Visit(GreaterNode node)
        { }

        public void Visit(VarNode node)
        { }

        public void Visit(NotInNode node)
        { }

        public void Visit(DiffNode node)
        { }

        public void Visit(InNode node)
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

        private void ReportHasMixedTypes(InNode node)
        {
            this.AddSemanticError(node.FullSpan, AnalysisMessage.MixedTypesNotAllowed, SemanticErrorKind.MixedValues);
        }

        public void Visit(AndNode node)
        { }

        public void Visit(RootScriptNode node)
        { }

        public void Visit(StartAtNode node)
        { }

        public void Visit(StopAtNode node)
        { }

        public void Visit(WordNode node)
        { }

        public void Visit(FunctionNode node)
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
