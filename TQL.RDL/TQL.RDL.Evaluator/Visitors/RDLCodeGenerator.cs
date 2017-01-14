using System;
using System.Collections.Generic;
using System.Linq;
using RDL.Parser.Helpers;
using TQL.RDL.Evaluator.Instructions;
using TQL.RDL.Parser;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Visitors
{
    public class RdlCodeGenerator : INodeVisitor
    {
        #region Private variables

        private DateTimeOffset _startAt;
        private DateTimeOffset? _stopAt;
        private readonly Stack<List<IRdlInstruction>> _functions;
        private MemoryVariables _variables;
        private Func<DateTimeOffset, DateTimeOffset> _generateNext;
        private readonly DefaultMethods _methods;
        private readonly Dictionary<string, int> _labels;
        private readonly RdlMetadata _metadatas;
        private RdlVirtualMachine _machine;

        #endregion

        #region Private static variables

        private static readonly string NDateTime = Nullable.GetUnderlyingType(typeof(DateTimeOffset?)).Name;
        private static readonly string NInt64 = Nullable.GetUnderlyingType(typeof(long?)).Name;
        private static readonly string NBoolean = Nullable.GetUnderlyingType(typeof(bool?)).Name;

        #endregion

        #region Protected Getters / Setters

        protected List<IRdlInstruction> Instructions => _functions.Peek();

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiate code generator object
        /// </summary>
        /// <param name="metadatas"></param>
        public RdlCodeGenerator(RdlMetadata metadatas)
            : this(metadatas, DateTimeOffset.UtcNow)
        { }

        /// <summary>
        /// Instantiate code generator object with custom startAt
        /// </summary>
        /// <param name="metadatas"></param>
        /// <param name="startAt"></param>
        private RdlCodeGenerator(RdlMetadata metadatas, DateTimeOffset startAt)
        {
            _methods = new DefaultMethods();
            _variables = new MemoryVariables();
            _functions = new Stack<List<IRdlInstruction>>();
            _functions.Push(new List<IRdlInstruction>());
            _stopAt = null;
            _labels = new Dictionary<string, int>();
            _metadatas = metadatas;
            _startAt = startAt;
        }

        #endregion


        public RdlVirtualMachine VirtualMachine => _machine;

        public virtual void Visit(WhereConditionsNode node)
        {

        }

        public virtual void Visit(StartAtNode node)
        {
            _startAt = node.When;
        }

        public virtual void Visit(StopAtNode node)
        {
            _stopAt = node.When;
        }

        public virtual void Visit(RepeatEveryNode node)
        {
            switch(node.DatePart)
            {
                case RepeatEveryNode.PartOfDate.Seconds:
                    _generateNext = (DateTimeOffset datetime) => datetime.AddSeconds(node.Value);
                    break;
                case RepeatEveryNode.PartOfDate.Minutes:
                    _generateNext = (DateTimeOffset datetime) => datetime.AddMinutes(node.Value);
                    break;
                case RepeatEveryNode.PartOfDate.Hours:
                    _generateNext = (DateTimeOffset datetime) => datetime.AddHours(node.Value);
                    break;
                case RepeatEveryNode.PartOfDate.DaysOfMonth:
                    _generateNext = (DateTimeOffset datetime) => datetime.AddDays(node.Value);
                    break;
                case RepeatEveryNode.PartOfDate.Months:
                    _generateNext = (DateTimeOffset datetime) => datetime.AddMonths(node.Value);
                    break;
                case RepeatEveryNode.PartOfDate.Years:
                    _generateNext = (DateTimeOffset datetime) => datetime.AddYears(node.Value);
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
            Instructions.Add(new PushDateTimeInstruction(node.DateTime));
        }

        public virtual void Visit(NotInNode node)
        {
            Visit(node as InNode);
            Instructions.Add(new NotInstruction());
        }

        public virtual void Visit(VarNode node)
        {
            switch(node.Token.Value)
            {
                case "second":
                    Instructions.Add(new LoadNumericVariableInstruction(v => v.Second));
                    break;
                case "minute":
                    Instructions.Add(new LoadNumericVariableInstruction(v => v.Minute));
                    break;
                case "hour":
                    Instructions.Add(new LoadNumericVariableInstruction(v => v.Hour));
                    break;
                case "day":
                    Instructions.Add(new LoadNumericVariableInstruction(v => v.Day));
                    break;
                case "month":
                    Instructions.Add(new LoadNumericVariableInstruction(v => v.Month));
                    break;
                case "year":
                    Instructions.Add(new LoadNumericVariableInstruction(v => v.Year));
                    break;
                case "current":
                    Instructions.Add(new LoadDateTimeVariableInstruction(v => v.Current));
                    break;
                case "weekday":
                    Instructions.Add(new LoadNumericVariableInstruction(v => (int)v.DayOfWeek));
                    break;
            }
        }

        public virtual void Visit(NumericNode node)
        {
            Instructions.Add(new PushNumericInstruction(node.Value));
        }

        public virtual void Visit(ArgListNode node)
        {
            Instructions.Add(new LoadToRegister(Registers.B, node.Descendants.Length));
        }

        public virtual void Visit(EqualityNode node)
        {
            ExpressionGenerateInstructions<EqualityNumeric>(node);
        }

        public virtual void Visit(DiffNode node)
        {
            ExpressionGenerateInstructions<DiffNumeric>(node);
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
            Instructions.Add(new Modify(
                (f) => f.Datetimes.Push(_generateNext(f.Datetimes.Pop()))));

            Instructions.Add(new BreakInstruction());

            _machine = new RdlVirtualMachine(_labels, _generateNext, Instructions.ToArray(), _stopAt, _startAt);
            _methods.SetMachine(_machine);
        }

        public virtual void Visit(WordNode node)
        {
            var value = 0;
            switch (node.Token.Value.ToLowerInvariant())
            {
                case "monday":
                    value = (int) DayOfWeek.Monday;
                    goto case "__addinstr__";
                case "tuesday":
                    value = (int) DayOfWeek.Tuesday;
                    goto case "__addinstr__";
                case "wednesday":
                    value = (int) DayOfWeek.Wednesday;
                    goto case "__addinstr__";
                case "thursday":
                    value = (int) DayOfWeek.Thursday;
                    goto case "__addinstr__";
                case "friday":
                    value = (int) DayOfWeek.Friday;
                    goto case "__addinstr__";
                case "saturday":
                    value = (int) DayOfWeek.Saturday;
                    goto case "__addinstr__";
                case "sunday":
                    value = (int) DayOfWeek.Sunday;
                    goto case "__addinstr__";
                case "__addinstr__":
                    Instructions.Add(new PushNumericInstruction(value));
                    break;
            }
        }

        public virtual void Visit(FunctionNode node)
        {
            var argTypes = node.Descendants.Select(f => f.ReturnType).ToArray();
            var registeredFunction = _metadatas.GetMethod(node.Name, argTypes);
            var returnName = registeredFunction.ReturnType.GetTypeName();

            object obj = null;
            if (registeredFunction.DeclaringType.Name == nameof(DefaultMethods))
            {
                obj = _methods;
            }

            Instructions.Add(new PrepareFunctionCall(argTypes));

            if (returnName == NDateTime)
            {
                Instructions.Add(new CallExternalDatetime(obj, registeredFunction));
            }
            else if(returnName == NInt64)
            {
                Instructions.Add(new CallExternalNumeric(obj, registeredFunction));
            }
            else if(returnName == NBoolean)
            {
                Instructions.Add(new CallExternalNumeric(obj, registeredFunction));
            }
        }

        public virtual void Visit(GreaterNode node)
        {
            ExpressionGenerateInstructions<GreaterNumeric>(node);
        }

        public virtual void Visit(GreaterEqualNode node)
        {
            ExpressionGenerateInstructions<GreaterEqualNumeric>(node);
        }

        public virtual void Visit(LessNode node)
        {
            ExpressionGenerateInstructions<LessNumeric>(node);
        }

        public virtual void Visit(LessEqualNode node)
        {
            ExpressionGenerateInstructions<LessEqualNumeric>(node);
        }

        public virtual void Visit(AddNode node)
        {
            ExpressionGenerateInstructions<AddNumericToNumeric>(node);
        }

        public virtual void Visit(ModuloNode node)
        {
            ExpressionGenerateInstructions<ModuloNumericToNumeric>(node);
        }

        public virtual void Visit(StarNode node)
        {
            Instructions.Add(new MultiplyNumerics());
        }

        public virtual void Visit(FSlashNode node)
        {
            if(node.Left.ReturnType == typeof(long) && node.Right.ReturnType == typeof(long))
            {
                Instructions.Add(new DivideNumeric());
            }
        }

        public virtual void Visit(HyphenNode node)
        {
            if (node.Left.ReturnType == typeof(long) && node.Right.ReturnType == typeof(long))
            {
                Instructions.Add(new SubtractNumeric());
            }
        }

        public virtual void Visit(CaseNode node)
        {

        }

        public virtual void Visit(WhenThenNode node)
        {
            _labels.Add($"when_{node.FullSpan.Start}{node.FullSpan.End}", Instructions.Count);
        }

        public virtual void Visit(WhenNode node)
        {
            var elseNode = node.Parent.Parent.Else;

            if (node.Parent.ArrayOrder == node.Parent.Parent.WhenThenExpressions.Count() - 1) //is last node. 
            {
                Instructions.Add(new JumpToLabelNotEqual($"else_{elseNode.FullSpan.Start}{elseNode.FullSpan.End}"));
            }
            else
            {
                var nextWhenNode = node.Parent.Parent.Descendants[node.Parent.ArrayOrder + 1];
                Instructions.Add(new JumpToLabelNotEqual($"when_{nextWhenNode.FullSpan.Start}{nextWhenNode.FullSpan.End}"));
            }
        }

        public virtual void Visit(ThenNode node)
        {
            var elseSpan = node.Parent.Parent.Else.FullSpan;
            Instructions.Add(new JumpToLabel($"esac_{elseSpan.Start}{elseSpan.End}"));

            if (node.Parent.ArrayOrder == node.Parent.Parent.WhenThenExpressions.Count() - 1)
            {
                _labels.Add($"else_{node.Parent.Parent.Else.FullSpan.Start}{node.Parent.Parent.Else.FullSpan.End}", Instructions.Count);
            }
        }

        public virtual void Visit(ElseNode node)
        {
            _labels.Add($"esac_{node.Parent.Else.FullSpan.Start}{node.Parent.Else.FullSpan.End}", Instructions.Count);
        }

        private void ExpressionGenerateIn<TOperator>(InNode node)
            where TOperator : IRdlInstruction, new()
        {
            Instructions.Add(new TOperator());
        }

        private void ExpressionGenerateLeftToRightInstructions<TOperator>(BinaryNode node)
            where TOperator: IRdlInstruction, new()
        {
            Instructions.Add(new TOperator());
        }

        private void ExpressionGenerateInstructions<TNumericOp>(BinaryNode node) where TNumericOp: IRdlInstruction, new ()
        {
            Instructions.Add(new TNumericOp());
        }
    }
}
