using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TQL.RDL.Evaluator.Instructions;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator
{

    public class RDLCodeGenerationVisitor : INodeVisitor
    {
        private DateTimeOffset? startAt;
        private DateTimeOffset? stopAt;
        private RDLVirtualMachine machine;
        private Stack<IRDLInstruction> instructions;
        private MemoryVariables variables;
        private Func<DateTimeOffset?, DateTimeOffset?> generateNext;

        public RDLCodeGenerationVisitor()
        {
            variables = new MemoryVariables();
            instructions = new Stack<IRDLInstruction>();
            startAt = DateTimeOffset.Now;
            stopAt = null;

            this.Register<RDLCodeGenerationVisitor>(f => true);
        }

        public void Register<T>(Expression<Func<T, bool>> exp)
        {
        }

        public RDLVirtualMachine VirtualMachine => new RDLVirtualMachine(generateNext, instructions.ToArray());

        public void Visit(WhereConditionsNode node)
        {
            for(int i = 0; i < node.Descendants.Length; ++i)
            {
                node.Descendants[i].Accept(this);
            }
        }

        public void Visit(StopAtNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(RepeatEveryNode node)
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

        public void Visit(OrNode node)
        {
            ExpressionGenerateLeftToRightInstructions<OrInstruction>(node);
        }

        public void Visit(DateTimeNode node)
        {
            instructions.Push(new PushDateTimeInstruction(node.DateTime));
        }

        public void Visit(CommaNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(NotNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(VarNode node)
        {
            switch(node.Token.Value)
            {
                case "second":
                    instructions.Push(new LoadNumericVariableInstruction(v => v.Second));
                    break;
                case "minute":
                    instructions.Push(new LoadNumericVariableInstruction(v => v.Minute));
                    break;
                case "hour":
                    instructions.Push(new LoadNumericVariableInstruction(v => v.Hour));
                    break;
                case "day":
                    instructions.Push(new LoadNumericVariableInstruction(v => v.Day));
                    break;
                case "month":
                    instructions.Push(new LoadNumericVariableInstruction(v => v.Month));
                    break;
                case "year":
                    instructions.Push(new LoadNumericVariableInstruction(v => v.Year));
                    break;
                case "current":
                    instructions.Push(new LoadDateTimeVariableInstruction(v => v.Current));
                    break;
                case "weekday":
                    instructions.Push(new LoadNumericVariableInstruction(v => (int?)v.Current?.DayOfWeek));
                    break;
            }
        }

        public void Visit(ConstantNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(NumericNode node)
        {
            instructions.Push(new PushNumericInstruction(node.Value));
        }

        public void Visit(ArgListNode node)
        {
            for (int i = node.Descendants.Length - 1; i >= 0; --i)
            {
                node.Descendants[i].Accept(this);
            }
        }

        public void Visit(EqualityNode node)
        {
            ExpressionGenerateInstructions<EqualityDatetime, EqualityNumeric>(node);
        }

        public void Visit(DiffNode node)
        {
            ExpressionGenerateInstructions<DiffDatetime, DiffNumeric>(node);
        }

        public void Visit(InNode node)
        {
            switch(node.ReturnType.Name)
            {
                case nameof(Int64):
                    ExpressionGenerateIn<InInstructionNumeric>(node);
                    break;
                case nameof(DateTimeOffset):
                    ExpressionGenerateIn<InInstructionDatetime>(node);
                    break;
            }
        }

        public void Visit(AndNode node)
        {
            ExpressionGenerateLeftToRightInstructions<AndInstruction>(node);
        }

        public void Visit(RootScript node)
        {
            instructions.Push(new BreakInstruction());
            instructions.Push(new Modify((f) => f.Datetimes.Push(generateNext(f.Datetimes.Pop()))));
            for (int i = 0; i < node.Descendants.Length; ++i)
            {
                node.Descendants[i].Accept(this);
            }
        }

        public void Visit(StartAtNode node)
        {
            machine.ReferenceTime = node.When;
        }

        public void Visit(WordNode node)
        {
            throw new NotImplementedException();
        }

        public void Visit(FunctionNode node)
        {
            foreach(var arg in node.Descendants)
            {
                arg.Accept(this);
            }
            instructions.Push(new PushNumericInstruction(node.Descendants.Length));
            instructions.Push(new CallExternalInstruction(null, node.Name));
        }

        public void Visit(GreaterNode node)
        {
            ExpressionGenerateInstructions<GreaterDatetime, GreaterNumeric>(node);
        }

        public void Visit(GreaterEqualNode node)
        {
            ExpressionGenerateInstructions<GreaterEqualDatetime, GreaterEqualNumeric>(node);
        }

        public void Visit(LessNode node)
        {
            ExpressionGenerateInstructions<LessDatetime, LessNumeric>(node);
        }

        public void Visit(LessEqualNode node)
        {
            ExpressionGenerateInstructions<LessEqualDatetime, LessEqualNumeric>(node);
        }

        private void ExpressionGenerateIn<TOperator>(InNode node)
            where TOperator : IRDLInstruction, new()
        {
            instructions.Push(new TOperator());
            instructions.Push(new PushNumericInstruction((node.Right as ArgListNode).Descendants.Length));
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        private void ExpressionGenerateLeftToRightInstructions<TOperator>(BinaryNode node)
            where TOperator: IRDLInstruction, new()
        {
            instructions.Push(new TOperator());
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        private void ExpressionGenerateRightToLeftInstructions<TOperator>(BinaryNode node)
            where TOperator : IRDLInstruction, new()
        {
            instructions.Push(new TOperator());
            node.Right.Accept(this);
            node.Left.Accept(this);
        }

        private void ExpressionGenerateInstructions<TDateTimeOp, TNumericOp>(BinaryNode node)
            where TDateTimeOp: IRDLInstruction, new ()
            where TNumericOp: IRDLInstruction, new ()
        {
            if (node.Left.IsLeaf)
            {
                switch (node.Left.ReturnType.Name)
                {
                    case nameof(DateTimeOffset):
                        instructions.Push(new TDateTimeOp());
                        break;
                    case nameof(Int64):
                        instructions.Push(new TNumericOp());
                        break;
                }
                node.Left.Accept(this);
                node.Right.Accept(this);
            }
            else if (node.Right.IsLeaf)
            {
                switch (node.Right.ReturnType.Name)
                {
                    case nameof(DateTimeOffset):
                        instructions.Push(new TDateTimeOp());
                        break;
                    case nameof(Int64):
                        instructions.Push(new TNumericOp());
                        break;
                }
                node.Left.Accept(this);
                node.Right.Accept(this);
            }
            else
            {
                instructions.Push(new TNumericOp());
                node.Left.Accept(this);
                node.Right.Accept(this);
            }
        }
    }
}
