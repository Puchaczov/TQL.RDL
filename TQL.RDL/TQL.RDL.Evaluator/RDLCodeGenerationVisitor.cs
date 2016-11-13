using RDL.Parser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TQL.RDL.Evaluator.Instructions;
using TQL.RDL.Parser;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator
{

    public class RDLCodeGenerationVisitor : INodeVisitor
    {
        private DateTimeOffset startAt;
        private DateTimeOffset? stopAt;
        private Stack<List<IRDLInstruction>> functions;
        private MemoryVariables variables;
        private Func<DateTimeOffset?, DateTimeOffset?> generateNext;
        private DefaultMethods methods;
        private Dictionary<string, int> labels;

        private static string nDateTime = Nullable.GetUnderlyingType(typeof(Nullable<DateTimeOffset>)).Name;
        private static string nInt64 = Nullable.GetUnderlyingType(typeof(Nullable<long>)).Name;
        private static string nBoolean = Nullable.GetUnderlyingType(typeof(Nullable<bool>)).Name;

        private static int numeric = 0;

        private RDLVirtualMachine machine;

        protected List<IRDLInstruction> instructions => functions.Peek();

        public RDLCodeGenerationVisitor()
        {
            methods = new DefaultMethods();
            variables = new MemoryVariables();
            functions = new Stack<List<IRDLInstruction>>();
            functions.Push(new List<IRDLInstruction>());
            startAt = DateTimeOffset.Now;
            stopAt = null;
            labels = new Dictionary<string, int>();
        }

        public RDLVirtualMachine VirtualMachine => machine;

        public virtual void Visit(WhereConditionsNode node)
        {
            for(int i = 0; i < node.Descendants.Length; ++i)
            {
                node.Descendants[i].Accept(this);
            }
        }

        public virtual void Visit(StopAtNode node)
        {
            stopAt = node.Datetime;
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
            for (int i = node.Descendants.Length - 1; i >= 0; --i)
            {
                node.Descendants[i].Accept(this);
            }
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
            switch(node.ReturnType.GetTypeName())
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
            for (int i = 0; i < node.Descendants.Length; ++i)
            {
                node.Descendants[i].Accept(this);
            }

            instructions.Add(new Modify(
                (f) => f.Datetimes.Push(generateNext(f.Datetimes.Pop()))));

            instructions.Add(new BreakInstruction());

            machine = new RDLVirtualMachine(labels, generateNext, instructions.ToArray(), stopAt, startAt);
            methods.SetMachine(machine);
        }

        public virtual void Visit(StartAtNode node)
        {
            startAt = node.When;
        }

        public virtual void Visit(WordNode node)
        {
            throw new NotImplementedException();
        }

        public virtual void Visit(FunctionNode node)
        {
            var argTypes = node.Descendants.Select(f => f.ReturnType).ToArray();
            var registeredFunction = GlobalMetadata.GetMethod(node.Name, argTypes);
            var returnName = registeredFunction.ReturnType.GetTypeName();

            object obj = null;
            if (registeredFunction.DeclaringType.Name == nameof(DefaultMethods))
            {
                obj = methods;
            }

            foreach (var arg in node.Descendants)
            {
                arg.Accept(this);
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
            node.Right.Accept(this);
            node.Left.Accept(this);
            instructions.Add(new AddNumericToNumeric());
        }

        public virtual void Visit(StarNode node)
        {
            node.Right.Accept(this);
            node.Left.Accept(this);
            instructions.Add(new MultiplyNumerics());
        }

        public virtual void Visit(FSlashNode node)
        {
            if(node.Left.ReturnType == typeof(Int64) && node.Right.ReturnType == typeof(Int64))
            {
                node.Right.Accept(this);
                node.Left.Accept(this);
                instructions.Add(new DivideNumeric());
            }
        }

        public virtual void Visit(HyphenNode node)
        {
            if (node.Left.ReturnType == typeof(Int64) && node.Right.ReturnType == typeof(Int64))
            {
                node.Right.Accept(this);
                node.Left.Accept(this);
                instructions.Add(new SubtractNumeric());
            }
        }

        public virtual void Visit(CaseNode node)
        {
            labels.Add($"case_when_label_start{numeric}", 0); //TO DO, USE CASE: 2x case, chyba nie moze zostac 0
            
            for (int i = 0, j = node.Expressions.Length - 1; i < j; ++i)
            {
                Visit(node.Expressions[i], numeric + i, numeric, false);
                labels.Add($"case_when_label_next{numeric + i}", instructions.Count);
            }

            Visit(node.Expressions[node.Expressions.Length - 1], numeric, numeric, true);

            labels.Add($"case_when_label_else{numeric}", instructions.Count);
            node.Else.Accept(this);

            labels.Add($"case_when_label_exit{numeric}", instructions.Count);
        }

        public virtual void Visit(WhenThenNode node)
        {
            throw new NotSupportedException();
        }

        private void Visit(WhenThenNode node, int labelNumber, int exitNumber, bool last)
        {
            node.When.Accept(this);
            if (!last)
            {
                instructions.Add(new JumpToLabelNotEqual($"case_when_label_next{labelNumber}"));
            }
            else
            {
                instructions.Add(new JumpToLabelNotEqual($"case_when_label_else{labelNumber}"));
            }
            node.Then.Accept(this);
            instructions.Add(new JumpToLabel($"case_when_label_exit{exitNumber}"));
        }

        public virtual void Visit(WhenNode node)
        {
            node.Expression.Accept(this);
        }

        public virtual void Visit(ThenNode node)
        {
            node.Descendant.Accept(this);
        }

        public virtual void Visit(ElseNode node)
        {
            node.Descendant.Accept(this);
        }

        private void ExpressionGenerateIn<TOperator>(InNode node)
            where TOperator : IRDLInstruction, new()
        {
            node.Right.Accept(this);
            node.Left.Accept(this);
            instructions.Add(new PushNumericInstruction((node.Right as ArgListNode).Descendants.Length));
            instructions.Add(new TOperator());
        }

        private void ExpressionGenerateLeftToRightInstructions<TOperator>(BinaryNode node)
            where TOperator: IRDLInstruction, new()
        {
            node.Right.Accept(this);
            node.Left.Accept(this);
            instructions.Add(new TOperator());
        }

        private void ExpressionGenerateRightToLeftInstructions<TOperator>(BinaryNode node)
            where TOperator : IRDLInstruction, new()
        {
            node.Right.Accept(this);
            node.Left.Accept(this);
            instructions.Add(new TOperator());
        }

        private void ExpressionGenerateInstructions<TDateTimeOp, TNumericOp>(BinaryNode node)
            where TDateTimeOp: IRDLInstruction, new ()
            where TNumericOp: IRDLInstruction, new ()
        {
            if (node.Left.IsLeaf)
            {
                node.Right.Accept(this);
                node.Left.Accept(this);
                switch (node.Left.ReturnType.GetTypeName())
                {
                    case nameof(DateTimeOffset):
                        instructions.Add(new TDateTimeOp());
                        break;
                    case nameof(Int64):
                        instructions.Add(new TNumericOp());
                        break;
                }
            }
            else if (node.Right.IsLeaf)
            {
                node.Right.Accept(this);
                node.Left.Accept(this);
                switch (node.Right.ReturnType.GetTypeName())
                {
                    case nameof(DateTimeOffset):
                        instructions.Add(new TDateTimeOp());
                        break;
                    case nameof(Int64):
                        instructions.Add(new TNumericOp());
                        break;
                }
            }
            else
            {
                node.Right.Accept(this);
                node.Left.Accept(this);
                instructions.Add(new TNumericOp());
            }
        }
    }
}
