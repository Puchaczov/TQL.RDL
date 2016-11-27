using RDL.Parser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TQL.RDL.Evaluator.Instructions;
using TQL.RDL.Parser;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator
{

    public class RDLCodeGenerator : RDLAnalyzerBase
    {
        private class CaseWhenOccurence
        {
            public int StatementDescendatsCount { get; set; }
            public int WhenThenCounter { get; set; }
        }

        private DateTimeOffset startAt;
        private DateTimeOffset? stopAt;
        private Stack<List<IRDLInstruction>> functions;
        private MemoryVariables variables;
        private Func<DateTimeOffset?, DateTimeOffset?> generateNext;
        private DefaultMethods methods;
        private Dictionary<string, int> labels;
        private RdlMetadata metadatas;

        private DateTimeOffset? minDate;
        private DateTimeOffset? maxDate;

        private static string nDateTime = Nullable.GetUnderlyingType(typeof(Nullable<DateTimeOffset>)).Name;
        private static string nInt64 = Nullable.GetUnderlyingType(typeof(Nullable<long>)).Name;
        private static string nBoolean = Nullable.GetUnderlyingType(typeof(Nullable<bool>)).Name;

        private int caseWhenCounter = 0;
        private Stack<CaseWhenOccurence> caseWhenOccurence = new Stack<CaseWhenOccurence>();

        private static string labelNamePattern = "label_case_when_";

        private RDLVirtualMachine machine;

        protected List<IRDLInstruction> instructions => functions.Peek();

        public RDLCodeGenerator(RdlMetadata metadatas, DateTimeOffset? minDate, DateTimeOffset? maxDate)
        {
            methods = new DefaultMethods();
            variables = new MemoryVariables();
            functions = new Stack<List<IRDLInstruction>>();
            functions.Push(new List<IRDLInstruction>());
            startAt = DateTimeOffset.Now;
            stopAt = null;
            labels = new Dictionary<string, int>();
            this.metadatas = metadatas;

            this.minDate = minDate;
            this.maxDate = maxDate;
        }

        public RDLVirtualMachine VirtualMachine => machine;

        public override void Visit(WhereConditionsNode node)
        {

        }

        public override void Visit(StartAtNode node)
        {
            if (minDate.HasValue && minDate.Value > node.When)
                startAt = minDate.Value;
            else
                startAt = node.When;
        }

        public override void Visit(StopAtNode node)
        {
            if (maxDate.HasValue && maxDate.Value < node.Datetime)
                stopAt = maxDate.Value;
            else
                stopAt = node.Datetime;
        }

        public override void Visit(RepeatEveryNode node)
        {
            switch(node.DatePart)
            {
                case RepeatEveryNode.PartOfDate.Seconds:
                    generateNext = (DateTimeOffset? datetime) => datetime.Value.AddSeconds(node.Value);
                    break;
                case RepeatEveryNode.PartOfDate.Minutes:
                    generateNext = (DateTimeOffset? datetime) => datetime.Value.AddMinutes(node.Value);
                    break;
                case RepeatEveryNode.PartOfDate.Hours:
                    generateNext = (DateTimeOffset? datetime) => datetime.Value.AddHours(node.Value);
                    break;
                case RepeatEveryNode.PartOfDate.DaysOfMonth:
                    generateNext = (DateTimeOffset? datetime) => datetime.Value.AddDays(node.Value);
                    break;
                case RepeatEveryNode.PartOfDate.Months:
                    generateNext = (DateTimeOffset? datetime) => datetime.Value.AddMonths(node.Value);
                    break;
                case RepeatEveryNode.PartOfDate.Years:
                    generateNext = (DateTimeOffset? datetime) => datetime.Value.AddYears(node.Value);
                    break;
            }
        }

        public override void Visit(NumericConsequentRepeatEveryNode node)
        {
            Visit(node as RepeatEveryNode);
        }

        public override void Visit(OrNode node)
        {
            ExpressionGenerateLeftToRightInstructions<OrInstruction>(node);
        }

        public override void Visit(DateTimeNode node)
        {
            instructions.Add(new PushDateTimeInstruction(node.DateTime));
        }

        public override void Visit(NotInNode node)
        {
            Visit(node as InNode);
            instructions.Add(new NotInstruction());
        }

        public override void Visit(VarNode node)
        {
            switch(node.Token.Value)
            {
                case "second":
                    instructions.Add(new LoadNumericVariableInstruction(v => v.Second));
                    break;
                case "minute":
                    instructions.Add(new LoadNumericVariableInstruction(v => v.Minute));
                    break;
                case "hour":
                    instructions.Add(new LoadNumericVariableInstruction(v => v.Hour));
                    break;
                case "day":
                    instructions.Add(new LoadNumericVariableInstruction(v => v.Day));
                    break;
                case "month":
                    instructions.Add(new LoadNumericVariableInstruction(v => v.Month));
                    break;
                case "year":
                    instructions.Add(new LoadNumericVariableInstruction(v => v.Year));
                    break;
                case "current":
                    instructions.Add(new LoadDateTimeVariableInstruction(v => v.Current));
                    break;
                case "weekday":
                    instructions.Add(new LoadNumericVariableInstruction(v => (int?)v.Current?.DayOfWeek));
                    break;
            }
        }

        public override void Visit(NumericNode node)
        {
            instructions.Add(new PushNumericInstruction(node.Value));
        }

        public override void Visit(ArgListNode node)
        {

        }

        public override void Visit(EqualityNode node)
        {
            ExpressionGenerateInstructions<EqualityDatetime, EqualityNumeric>(node);
        }

        public override void Visit(DiffNode node)
        {
            ExpressionGenerateInstructions<DiffDatetime, DiffNumeric>(node);
        }

        public override void Visit(InNode node)
        {
            switch(node.Left.ReturnType.GetTypeName())
            {
                case nameof(Int64):
                    ExpressionGenerateIn<InInstructionNumeric>(node);
                    break;
                case nameof(DateTimeOffset):
                    ExpressionGenerateIn<InInstructionDatetime>(node);
                    break;
            }
        }

        public override void Visit(AndNode node)
        {
            ExpressionGenerateLeftToRightInstructions<AndInstruction>(node);
        }

        public override void Visit(RootScriptNode node)
        {
            base.Visit(node);

            instructions.Add(new Modify(
                (f) => f.Datetimes.Push(generateNext(f.Datetimes.Pop()))));

            instructions.Add(new BreakInstruction());

            if (!node.Descendants.OfType<StartAtNode>().Any() && minDate.HasValue)
            {
                startAt = minDate.Value;
            }

            if (!node.Descendants.OfType<StopAtNode>().Any() && maxDate.HasValue)
            {
                stopAt = maxDate;
            }

            machine = new RDLVirtualMachine(labels, generateNext, instructions.ToArray(), stopAt, startAt);
            methods.SetMachine(machine);
        }

        public override void Visit(WordNode node)
        {
            throw new NotImplementedException();
        }

        public override void Visit(FunctionNode node)
        {
            var argTypes = node.Descendants.Select(f => f.ReturnType).ToArray();
            var registeredFunction = metadatas.GetMethod(node.Name, argTypes);
            var returnName = registeredFunction.ReturnType.GetTypeName();

            object obj = null;
            if (registeredFunction.DeclaringType.Name == nameof(DefaultMethods))
            {
                obj = methods;
            }

            instructions.Add(new PrepareFunctionCall(argTypes));

            if (returnName == nDateTime)
            {
                instructions.Add(new CallExternalDatetime(obj, registeredFunction));
            }
            else if(returnName == nInt64)
            {
                instructions.Add(new CallExternalNumeric(obj, registeredFunction));
            }
            else if(returnName == nBoolean)
            {
                instructions.Add(new CallExternalNumeric(obj, registeredFunction));
            }
        }

        public override void Visit(GreaterNode node)
        {
            ExpressionGenerateInstructions<GreaterDatetime, GreaterNumeric>(node);
        }

        public override void Visit(GreaterEqualNode node)
        {
            ExpressionGenerateInstructions<GreaterEqualDatetime, GreaterEqualNumeric>(node);
        }

        public override void Visit(LessNode node)
        {
            ExpressionGenerateInstructions<LessDatetime, LessNumeric>(node);
        }

        public override void Visit(LessEqualNode node)
        {
            ExpressionGenerateInstructions<LessEqualDatetime, LessEqualNumeric>(node);
        }

        public override void Visit(AddNode node)
        {
            ExpressionGenerateInstructions<AddNumericToDatetime, AddNumericToNumeric>(node);
        }

        public override void Visit(ModuloNode node)
        {
            instructions.Add(new AddNumericToNumeric());
        }

        public override void Visit(StarNode node)
        {
            instructions.Add(new MultiplyNumerics());
        }

        public override void Visit(FSlashNode node)
        {
            if(node.Left.ReturnType == typeof(Int64) && node.Right.ReturnType == typeof(Int64))
            {
                instructions.Add(new DivideNumeric());
            }
        }

        public override void Visit(HyphenNode node)
        {
            if (node.Left.ReturnType == typeof(Int64) && node.Right.ReturnType == typeof(Int64))
            {
                instructions.Add(new SubtractNumeric());
            }
        }

        public override void Visit(CaseNode node)
        {
        }

        public override void Visit(WhenThenNode node)
        {
            labels.Add($"{labelNamePattern}when_{node.Parent.FullSpan.Start}{node.ArrayOrder + 1}", this.instructions.Count);
        }

        public override void Visit(WhenNode node)
        {
            var caseSpan = node.Parent.Parent.FullSpan;
            if (node.Parent.ArrayOrder == node.Parent.Parent.WhenThenExpressions.Count() - 1) //is last node. 
            {
                instructions.Add(new JumpToLabel($"{labelNamePattern}else_{caseSpan.Start}{caseSpan.End}"));
            }
            else
            {
                instructions.Add(new JumpToLabelNotEqual($"{labelNamePattern}when_{node.Parent.FullSpan.Start}{node.Parent.ArrayOrder + 1}"));
            }
        }

        public override void Visit(ThenNode node)
        {
            var caseSpan = node.Parent.Parent.FullSpan;
            instructions.Add(new JumpToLabel($"{labelNamePattern}esac_{caseSpan.Start}{caseSpan.End}"));
        }

        public override void Visit(ElseNode node)
        {
            labels.Add($"{labelNamePattern}else_{node.Parent.FullSpan.Start}{node.Parent.FullSpan.End}", labels.Count);
        }

        private void ExpressionGenerateIn<TOperator>(InNode node)
            where TOperator : IRDLInstruction, new()
        {
            instructions.Add(new PushNumericInstruction((node.Right as ArgListNode).Descendants.Length));
            instructions.Add(new TOperator());
        }

        private void ExpressionGenerateLeftToRightInstructions<TOperator>(BinaryNode node)
            where TOperator: IRDLInstruction, new()
        {
            instructions.Add(new TOperator());
        }

        private void ExpressionGenerateRightToLeftInstructions<TOperator>(BinaryNode node)
            where TOperator : IRDLInstruction, new()
        {
            instructions.Add(new TOperator());
        }

        private void ExpressionGenerateInstructions<TDateTimeOp, TNumericOp>(BinaryNode node)
            where TDateTimeOp: IRDLInstruction, new ()
            where TNumericOp: IRDLInstruction, new ()
        {
            instructions.Add(new TNumericOp());
        }
    }
}
