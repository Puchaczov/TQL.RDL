using RDL.Parser.Helpers;
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
        {
            this.ReportReturnTypesAreNotSame(node, "Or");
        }

        public override void Visit(DateTimeNode node)
        {
            try
            {
                var temp = node.DateTime;
            }
            catch(Exception e)
            {
                this.criticalErrors.Add(e);
            }
        }

        public override void Visit(EqualityNode node)
        {
            this.ReportReturnTypesAreNotSame(node, "Equality");
        }

        public override void Visit(ArgListNode node)
        { }

        public override void Visit(NumericNode node)
        {
        }

        public override void Visit(GreaterEqualNode node)
        {
            this.ReportReturnTypesAreNotSame(node, "GreaterEqual");
        }

        public override void Visit(LessEqualNode node)
        {
            this.ReportReturnTypesAreNotSame(node, "LessEqual");
        }

        public override void Visit(AddNode node)
        {
            this.ReportReturnTypesAreNotSame(node, "Add");
        }

        public override void Visit(ModuloNode node)
        {
            this.ReportReturnTypesAreNotSame(node, "Modulo");
        }

        public override void Visit(FSlashNode node)
        {
            this.ReportReturnTypesAreNotSame(node, "Divide");
        }

        public override void Visit(ThenNode node)
        {
            try
            {
                throw new NotImplementedException(nameof(ThenNode));
            }
            catch (Exception exc)
            {
                criticalErrors.Add(exc);
            }
        }

        public override void Visit(CaseNode node)
        {
            try
            {
                throw new NotImplementedException(nameof(CaseNode));
            }
            catch(Exception exc)
            {
                criticalErrors.Add(exc);
            }
        }

        public override void Visit(WhenThenNode node)
        {
            try
            {
                throw new NotImplementedException(nameof(WhenThenNode));
            }
            catch (Exception exc)
            {
                criticalErrors.Add(exc);
            }
        }

        public override void Visit(ElseNode node)
        {
            try
            {
                throw new NotImplementedException(nameof(ElseNode));
            }
            catch (Exception exc)
            {
                criticalErrors.Add(exc);
            }
        }

        public override void Visit(WhenNode node)
        {
            try
            {
                throw new NotImplementedException(nameof(WhenNode));
            }
            catch (Exception exc)
            {
                criticalErrors.Add(exc);
            }
        }

        public override void Visit(StarNode node)
        {
            this.ReportReturnTypesAreNotSame(node, "Star");
        }

        public override void Visit(HyphenNode node)
        {
            this.ReportReturnTypesAreNotSame(node, "Hyphen");
        }

        public override void Visit(NumericConsequentRepeatEveryNode node)
        { }

        public override void Visit(LessNode node)
        {
            this.ReportReturnTypesAreNotSame(node, "Less");
        }

        public override void Visit(GreaterNode node)
        {
            this.ReportReturnTypesAreNotSame(node, "Greater");
        }

        public override void Visit(VarNode node)
        { }

        public override void Visit(NotInNode node)
        {
            this.Visit(node);
        }

        public override void Visit(DiffNode node)
        {
            this.ReportReturnTypesAreNotSame(node, "Diff");
        }

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
        {
            this.ReportReturnTypesAreNotSame(node, "And");
        }

        public override void Visit(StartAtNode node)
        {
            try
            {
                var tmp = node.When;
            }
            catch(Exception exc)
            {
                criticalErrors.Add(exc);
            }
        }

        public override void Visit(StopAtNode node)
        {
            try
            {
                var tmp = node.Datetime;
            }
            catch (Exception exc)
            {
                criticalErrors.Add(exc);
            }
        }

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

        private void ReportReturnTypesAreNotSame(BinaryNode node, string nodeName)
        {
            var left = node.Left.ReturnType.GetUnderlyingNullable();
            var right = node.Right.ReturnType.GetUnderlyingNullable();
            if (left != right)
            {
                this.AddSyntaxError(node.FullSpan, string.Format(AnalysisMessage.ReturnTypesAreNotTheSame, nodeName, left.Name, right.Name), SyntaxErrorKind.ImproperType);
            }
        }
    }
}
