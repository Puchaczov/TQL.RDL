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
            this.ReportTypeError(node, typeof(Boolean), "GreaterEqual");
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
            this.ReportTypeError(node, typeof(Boolean), "Equality");
        }

        public override void Visit(ArgListNode node)
        { }

        public override void Visit(NumericNode node)
        {
        }

        public override void Visit(GreaterEqualNode node)
        {
            this.ReportTypeError(node.Left, typeof(Boolean), "GreaterEqual");
            this.ReportTypeError(node.Right, typeof(Boolean), "GreaterEqual");
        }

        public override void Visit(LessEqualNode node)
        {
            this.ReportTypeError(node.Left, typeof(Boolean), "LessEqual");
            this.ReportTypeError(node.Right, typeof(Boolean), "LessEqual");
        }

        public override void Visit(AddNode node)
        {
            this.ReportTypeError(node.Left, typeof(Int64), "Add");
            this.ReportTypeError(node.Right, typeof(Int64), "Add");
        }

        public override void Visit(ModuloNode node)
        {
            this.ReportTypeError(node.Left, typeof(Int64), "Modulo");
            this.ReportTypeError(node.Right, typeof(Int64), "Modulo");
        }

        public override void Visit(FSlashNode node)
        {
            this.ReportTypeError(node.Left, typeof(Int64), "Divide");
            this.ReportTypeError(node.Right, typeof(Int64), "Divide");
        }

        public override void Visit(ThenNode node)
        { }

        public override void Visit(CaseNode node)
        { }

        public override void Visit(WhenThenNode node)
        { }

        public override void Visit(ElseNode node)
        { }

        public override void Visit(WhenNode node)
        {
            this.ReportTypeError(node, typeof(Boolean), "When");
        }

        public override void Visit(StarNode node)
        {
            this.ReportTypeError(node.Left, typeof(Int64), "Star");
            this.ReportTypeError(node.Right, typeof(Int64), "Star");
        }

        public override void Visit(HyphenNode node)
        {
            this.ReportTypeError(node.Left, typeof(Int64), "Hyphen");
            this.ReportTypeError(node.Right, typeof(Int64), "Hyphen");
        }

        public override void Visit(NumericConsequentRepeatEveryNode node)
        { }

        public override void Visit(LessNode node)
        {
            this.ReportTypeError(node.Left, typeof(Boolean), "Less");
            this.ReportTypeError(node.Right, typeof(Boolean), "Less");
        }

        public override void Visit(GreaterNode node)
        {
            this.ReportTypeError(node.Left, typeof(Boolean), "Greater");
            this.ReportTypeError(node.Right, typeof(Boolean), "Greater");
        }

        public override void Visit(VarNode node)
        { }

        public override void Visit(NotInNode node)
        {
            this.Visit(node);
        }

        public override void Visit(DiffNode node)
        {
            this.ReportTypeError(node.Left, typeof(Boolean), "Diff");
            this.ReportTypeError(node.Right, typeof(Boolean), "Diff");
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
            this.ReportTypeError(node.Left, typeof(Boolean), "and");
            this.ReportTypeError(node.Right, typeof(Boolean), "and");
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

        private void ReportTypeError(RdlSyntaxNode node, Type expectedType, string nodeName)
        {
            if (node.ReturnType != expectedType)
            {
                this.AddSyntaxError(node.FullSpan, string.Format(AnalysisMessage.TypeNotAllowed, typeof(Boolean).Name, nodeName), SyntaxErrorKind.ImproperType);
            }
        }

        private void ReportReturnTypesAreNotSame(BinaryNode node, string nodeName)
        {
            if(node.Left.ReturnType != node.Right.ReturnType)
            {
                this.AddSyntaxError(node.FullSpan, string.Format(AnalysisMessage.ReturnTypesAreNotTheSame, nodeName, node.Left.ReturnType.Name), SyntaxErrorKind.ImproperType);
            }
        }
    }
}
