using System;
using System.Collections.Generic;
using System.Linq;
using RDL.Parser.Helpers;
using TQL.Core.Tokens;
using TQL.RDL.Parser;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Visitors
{
    public sealed class RdlQueryValidator : AnalyzerBase
    {
        private readonly List<VisitationMessage> _errors;

        public IEnumerable<VisitationMessage> Errors => E;

        private readonly List<Exception> CriticalErrors;
        private IReadOnlyList<VisitationMessage> E => _errors.Concat(CriticalErrors.Select(f => new FatalVisitError(f))).ToArray();

        private RdlMetadata _metadatas;

        public bool IsValid => CriticalErrors.Count == 0 && _errors.Count == 0;

        private bool _startAtOccured = false;

        public RdlQueryValidator(RdlMetadata metadatas)
        {
            CriticalErrors = new List<Exception>();
            _errors = new List<VisitationMessage>();
            _metadatas = metadatas;
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
                CriticalErrors.Add(e);
            }
        }

        public override void Visit(OrNode node)
        {
            ReportReturnTypesAreNotSame(node, "Or");
        }

        public override void Visit(DateTimeNode node)
        {
            try
            {
                var temp = node.DateTime;
            }
            catch(Exception e)
            {
                CriticalErrors.Add(e);
            }
        }

        public override void Visit(EqualityNode node)
        {
            ReportReturnTypesAreNotSame(node, "Equality");
        }

        public override void Visit(ArgListNode node)
        {

        }

        public override void Visit(NumericNode node)
        {
        }

        public override void Visit(GreaterEqualNode node)
        {
            ReportReturnTypesAreNotSame(node, "GreaterEqual");
        }

        public override void Visit(LessEqualNode node)
        {
            ReportReturnTypesAreNotSame(node, "LessEqual");
        }

        public override void Visit(AddNode node)
        {
            ReportReturnTypesAreNotSame(node, "Add");
        }

        public override void Visit(ModuloNode node)
        {
            ReportReturnTypesAreNotSame(node, "Modulo");
        }

        public override void Visit(FSlashNode node)
        {
            ReportReturnTypesAreNotSame(node, "Divide");
        }

        public override void Visit(ThenNode node)
        {
            if(node.Descendants.Length == 0)
            {
                ReportLackOfThenExpression(node);
            }
        }

        public override void Visit(CaseNode node)
        {
            if(node.WhenThenExpressions.Length == 0)
            {
                ReportLackOfWhenThenExpression(node);
            }
        }

        public override void Visit(WhenThenNode node)
        { }

        public override void Visit(ElseNode node)
        {
            if(node.Descendants.Length == 0)
            {
                ReportLackOfElseReturnExpression(node);
            }
        }

        public override void Visit(WhenNode node)
        {
            if(node.Descendants.Length == 0)
            {
                ReportLackOfWhenReturnExpression(node);
            }
        }

        public override void Visit(StarNode node)
        {
            ReportReturnTypesAreNotSame(node, "Star");
        }

        public override void Visit(HyphenNode node)
        {
            ReportReturnTypesAreNotSame(node, "Hyphen");
        }

        public override void Visit(NumericConsequentRepeatEveryNode node)
        { }

        public override void Visit(LessNode node)
        {
            ReportReturnTypesAreNotSame(node, "Less");
        }

        public override void Visit(GreaterNode node)
        {
            ReportReturnTypesAreNotSame(node, "Greater");
        }

        public override void Visit(VarNode node)
        { }

        public override void Visit(NotInNode node)
        {
            Visit(node);
        }

        public override void Visit(DiffNode node)
        {
            ReportReturnTypesAreNotSame(node, "Diff");
        }

        public override void Visit(InNode node)
        {
            try
            {
                var dstType = node.Left.ReturnType.GetUnderlyingNullable();

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
                CriticalErrors.Add(e);
            }
        }

        public override void Visit(AndNode node)
        {
            ReportReturnTypesAreNotSame(node, "And");
        }

        public override void Visit(StartAtNode node)
        {
            try
            {
                _startAtOccured = true;
            }
            catch(Exception exc)
            {
                CriticalErrors.Add(exc);
            }
        }

        public override void Visit(StopAtNode node)
        {
            try
            {
                var tmp = node.When;
            }
            catch (Exception exc)
            {
                CriticalErrors.Add(exc);
            }
        }

        public override void Visit(WordNode node)
        { }

        public override void Visit(FunctionNode node)
        {
            try
            {
                if (!_metadatas.HasMethod(node.Name, node.Args.Descendants.Select(f => f.ReturnType).ToArray()))
                {
                    ReportUnknownFunctionCall(node);
                }
            }
            catch (Exception e)
            {
                CriticalErrors.Add(e);
            }
        }

        public override void Visit(RootScriptNode node)
        {
            if(!_startAtOccured)
            {
                ReportStartAtRequired();
            }
        }

        private void ReportStartAtRequired()
        {
            AddSyntaxError(new TextSpan(-1, 0), AnalysisMessage.StartAtRequired, SyntaxErrorKind.MissingValue);
        }

        private void ReportHasMixedTypes(InNode node)
        {
            AddSemanticError(node.FullSpan, AnalysisMessage.MixedTypesNotAllowed, SemanticErrorKind.MixedValues);
        }

        private void ReportUnknownFunctionCall(FunctionNode node)
        {
            AddSyntaxError(node.FullSpan, string.Format(AnalysisMessage.UnknownFunctionCall, node.Name, node.Args.Descendants.Select(f => f.ReturnType.Name).ToArray()), SyntaxErrorKind.UnsupportedFunctionCall);
        }

        private void ReportUnknownRepeatEveryDatePartFraction(RepeatEveryNode node)
        {
            AddSyntaxError(node.FullSpan, string.Format(AnalysisMessage.RepeateEveryContainsUnknownDatePartFraction, node.Token.Value), SyntaxErrorKind.WrongKeyword);
        }

        private void AddSemanticError(TextSpan span, string message, SemanticErrorKind kind)
        {
            AddSemanticError(new TextSpan[] { span }, message, kind);
        }

        private void AddSemanticError(TextSpan[] spans, string message, SemanticErrorKind kind)
        {
            _errors.Add(new SemanticError(spans, message, kind));
        }

        private void AddSyntaxError(TextSpan fullSpan, string v, SyntaxErrorKind missingValue)
        {
            _errors.Add(new SyntaxError(fullSpan, v, missingValue));
        }

        private void ReportReturnTypesAreNotSame(BinaryNode node, string nodeName)
        {
            var left = node.Left.ReturnType.GetUnderlyingNullable();
            var right = node.Right.ReturnType.GetUnderlyingNullable();
            if (left != right)
            {
                AddSyntaxError(node.FullSpan, string.Format(AnalysisMessage.ReturnTypesAreNotTheSame, nodeName, left.Name, right.Name), SyntaxErrorKind.ImproperType);
            }
        }

        private void ReportLackOfWhenThenExpression(CaseNode node)
        {
            AddSyntaxError(node.FullSpan, string.Format(AnalysisMessage.LackOfWhenThenExpression, node), SyntaxErrorKind.LackOfExpression);
        }

        private void ReportLackOfThenExpression(ThenNode node)
        {
            AddSyntaxError(node.FullSpan, string.Format(AnalysisMessage.LackOfThenReturnExpression, node), SyntaxErrorKind.LackOfExpression);
        }

        private void ReportLackOfElseReturnExpression(ElseNode node)
        {
            AddSyntaxError(node.FullSpan, string.Format(AnalysisMessage.LackOfElseReturnExpression, node.Parent), SyntaxErrorKind.LackOfExpression);
        }

        private void ReportLackOfWhenReturnExpression(WhenNode node)
        {
            AddSyntaxError(node.FullSpan, string.Format(AnalysisMessage.LackOfWhenReturnExpression, node.Parent), SyntaxErrorKind.LackOfExpression);
        }

        private void ReportArgListIsEmpty(ArgListNode node)
        {
            AddSyntaxError(node.FullSpan, string.Format(AnalysisMessage.ArgListCannotBeEmpty), SyntaxErrorKind.LackOfExpression);
        }
    }
}
