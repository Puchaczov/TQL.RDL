using RDL.Parser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TQL.RDL.Evaluator.Instructions;
using TQL.RDL.Parser;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator
{
    public class RDLCodeGenerator : INodeVisitor
    {
        private DateTimeOffset startAt;
        private DateTimeOffset? stopAt;
        private Stack<List<IRDLInstruction>> functions;
        private MemoryVariables variables;
        private Func<DateTimeOffset?, DateTimeOffset?> generateNext;
        private DefaultMethods methods;
        private Dictionary<string, int> labels;
        private RdlMetadata metadatas;

        private static string nDateTime = Nullable.GetUnderlyingType(typeof(Nullable<DateTimeOffset>)).Name;
        private static string nInt64 = Nullable.GetUnderlyingType(typeof(Nullable<long>)).Name;
        private static string nBoolean = Nullable.GetUnderlyingType(typeof(Nullable<bool>)).Name;

        private static string labelNamePattern = "label_case_when_";

        private RDLVirtualMachine machine;

        protected List<IRDLInstruction> instructions => functions.Peek();

        public RDLCodeGenerator(RdlMetadata metadatas)
            : this(metadatas, DateTimeOffset.UtcNow)
        {
        }

        public RDLCodeGenerator(RdlMetadata metadatas, DateTimeOffset startAt)
        {
            methods = new DefaultMethods();
            variables = new MemoryVariables();
            functions = new Stack<List<IRDLInstruction>>();
            functions.Push(new List<IRDLInstruction>());
            stopAt = null;
            labels = new Dictionary<string, int>();
            this.metadatas = metadatas;
            this.startAt = startAt;
        }

        public RDLVirtualMachine VirtualMachine => machine;

        public virtual void Visit(WhereConditionsNode node)
        {

        }

        public virtual void Visit(StartAtNode node)
        {
            startAt = node.When;
        }

        public virtual void Visit(StopAtNode node)
        {
            stopAt = node.When;
        }

        public virtual void Visit(RepeatEveryNode node)
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

        public virtual void Visit(NumericConsequentRepeatEveryNode node)
        {
            Visit(node as RepeatEveryNode);
        }

        public virtual void Visit(OrNode node)
        {
            ExpressionGenerateLeftToRightInstructions<OrInstruction>(node);
        }

        public virtual void Visit(DateTimeNode node)
        {
            instructions.Add(new PushDateTimeInstruction(node.DateTime));
        }

        public virtual void Visit(NotInNode node)
        {
            Visit(node as InNode);
            instructions.Add(new NotInstruction());
        }

        public virtual void Visit(VarNode node)
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

        public virtual void Visit(NumericNode node)
        {
            instructions.Add(new PushNumericInstruction(node.Value));
        }

        public virtual void Visit(ArgListNode node)
        {
            instructions.Add(new LoadToRegister(Registers.B, node.Descendants.Length));
        }

        public virtual void Visit(EqualityNode node)
        {
            ExpressionGenerateInstructions<EqualityDatetime, EqualityNumeric>(node);
        }

        public virtual void Visit(DiffNode node)
        {
            ExpressionGenerateInstructions<DiffDatetime, DiffNumeric>(node);
        }

        public virtual void Visit(InNode node)
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

        public virtual void Visit(AndNode node)
        {
            ExpressionGenerateLeftToRightInstructions<AndInstruction>(node);
        }

        public virtual void Visit(RootScriptNode node)
        {
            instructions.Add(new Modify(
                (f) => f.Datetimes.Push(generateNext(f.Datetimes.Pop()))));

            instructions.Add(new BreakInstruction());

            machine = new RDLVirtualMachine(labels, generateNext, instructions.ToArray(), stopAt, startAt);
            methods.SetMachine(machine);
        }

        public virtual void Visit(WordNode node)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(FunctionNode node)
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

        public virtual void Visit(GreaterNode node)
        {
            ExpressionGenerateInstructions<GreaterDatetime, GreaterNumeric>(node);
        }

        public virtual void Visit(GreaterEqualNode node)
        {
            ExpressionGenerateInstructions<GreaterEqualDatetime, GreaterEqualNumeric>(node);
        }

        public virtual void Visit(LessNode node)
        {
            ExpressionGenerateInstructions<LessDatetime, LessNumeric>(node);
        }

        public virtual void Visit(LessEqualNode node)
        {
            ExpressionGenerateInstructions<LessEqualDatetime, LessEqualNumeric>(node);
        }

        public virtual void Visit(AddNode node)
        {
            ExpressionGenerateInstructions<AddNumericToDatetime, AddNumericToNumeric>(node);
        }

        public virtual void Visit(ModuloNode node)
        {
            ExpressionGenerateInstructions<ModuloNumericToNumeric, ModuloNumericToNumeric>(node);
        }

        public virtual void Visit(StarNode node)
        {
            instructions.Add(new MultiplyNumerics());
        }

        public virtual void Visit(FSlashNode node)
        {
            if(node.Left.ReturnType == typeof(Int64) && node.Right.ReturnType == typeof(Int64))
            {
                instructions.Add(new DivideNumeric());
            }
        }

        public virtual void Visit(HyphenNode node)
        {
            if (node.Left.ReturnType == typeof(Int64) && node.Right.ReturnType == typeof(Int64))
            {
                instructions.Add(new SubtractNumeric());
            }
        }

        public virtual void Visit(CaseNode node)
        {

        }

        public virtual void Visit(WhenThenNode node)
        {
            labels.Add($"when_{node.FullSpan.Start}{node.FullSpan.End}", instructions.Count);
        }

        public virtual void Visit(WhenNode node)
        {
            var elseNode = node.Parent.Parent.Else;

            if (node.Parent.ArrayOrder == node.Parent.Parent.WhenThenExpressions.Count() - 1) //is last node. 
            {
                instructions.Add(new JumpToLabelNotEqual($"else_{elseNode.FullSpan.Start}{elseNode.FullSpan.End}"));
            }
            else
            {
                var nextWhenNode = node.Parent.Parent.Descendants[node.Parent.ArrayOrder + 1];
                instructions.Add(new JumpToLabelNotEqual($"when_{nextWhenNode.FullSpan.Start}{nextWhenNode.FullSpan.End}"));
            }
        }

        public virtual void Visit(ThenNode node)
        {
            var elseSpan = node.Parent.Parent.Else.FullSpan;
            instructions.Add(new JumpToLabel($"esac_{elseSpan.Start}{elseSpan.End}"));

            if (node.Parent.ArrayOrder == node.Parent.Parent.WhenThenExpressions.Count() - 1)
            {
                labels.Add($"else_{node.Parent.Parent.Else.FullSpan.Start}{node.Parent.Parent.Else.FullSpan.End}", instructions.Count);
            }
        }

        public virtual void Visit(ElseNode node)
        {
            labels.Add($"esac_{node.Parent.Else.FullSpan.Start}{node.Parent.Else.FullSpan.End}", instructions.Count);
        }

        private void ExpressionGenerateIn<TOperator>(InNode node)
            where TOperator : IRDLInstruction, new()
        {
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
