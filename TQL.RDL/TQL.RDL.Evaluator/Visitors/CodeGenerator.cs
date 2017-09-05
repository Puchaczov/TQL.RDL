using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using TQL.RDL.Evaluator.Instructions;
using TQL.RDL.Parser;
using TQL.RDL.Parser.Helpers;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Visitors
{
    public class CodeGenerator : INodeVisitor
    {
        #region Protected Getters / Setters

        /// <summary>
        ///     Gets the list that can store generated instructions.
        /// </summary>
        protected List<IRdlInstruction> Instructions => _functions.Peek();

        #endregion

        /// <summary>
        ///     Creates virtual machine instance.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Virtual machine object.</returns>
        public RdlVirtualMachine CreateVirtualMachine(CancellationToken token)
        {
            return new RdlVirtualMachine(_labels, Instructions.ToArray(), _stopAt, _startAt, _hasWhereConditions, token);
        }

        /// <summary>
        ///     Performs "WHERE" specific operations.
        /// </summary>
        /// <param name="node">The WhereConditions node.</param>
        public virtual void Visit(WhereConditionsNode node)
        {
            _hasWhereConditions = true;
        }

        /// <summary>
        ///     Performs "StartAt" specific operations.
        /// </summary>
        /// <param name="node">The StartAt node.</param>
        public virtual void Visit(StartAtNode node)
        {
            _startAt = node.When;
        }

        /// <summary>
        ///     Performs "StopAt" specific operations.
        /// </summary>
        /// <param name="node">The StopAt node.</param>
        public virtual void Visit(StopAtNode node)
        {
            _stopAt = node.When;
        }

        /// <summary>
        ///     Performs "RepeatEvery" specific operations.
        /// </summary>
        /// <param name="node">The RepeatEvery node.</param>
        public virtual void Visit(RepeatEveryNode node)
        {
            switch (node.DatePart)
            {
                case PartOfDate.Seconds:
                    _generateNext = datetime => datetime.AddSeconds(node.Value);
                    break;
                case PartOfDate.Minutes:
                    _generateNext = datetime => datetime.AddMinutes(node.Value);
                    break;
                case PartOfDate.Hours:
                    _generateNext = datetime => datetime.AddHours(node.Value);
                    break;
                case PartOfDate.DaysOfMonth:
                    _generateNext = datetime => datetime.AddDays(node.Value);
                    break;
                case PartOfDate.Months:
                    _generateNext = datetime => datetime.AddMonths(node.Value);
                    break;
                case PartOfDate.Years:
                    _generateNext = datetime => datetime.AddYears(node.Value);
                    break;
            }
            _partOfDate = node.DatePart;
        }

        /// <summary>
        ///     Performs "NumericConsequentRepeatEvery" specific operations.
        /// </summary>
        /// <param name="node">The NumericConsequentRepeatEvery node.</param>
        public virtual void Visit(NumericConsequentRepeatEveryNode node)
        {
            Visit(node as RepeatEveryNode);
        }

        /// <summary>
        ///     Performs "Or" specific operations.
        /// </summary>
        /// <param name="node">The "Or" node.</param>
        public virtual void Visit(OrNode node)
        {
            ExpressionGenerateLeftToRightInstructions<OrInstruction>(node);
        }

        /// <summary>
        ///     Performs "DateTime" specific operations.
        /// </summary>
        /// <param name="node">The "DateTime" node.</param>
        public virtual void Visit(DateTimeNode node)
        {
            Instructions.Add(new PushDateTimeInstruction(node.DateTime));
        }

        /// <summary>
        ///     Performs "NotIn" specific operations.
        /// </summary>
        /// <param name="node">The "NotIn" node.</param>
        public virtual void Visit(NotInNode node)
        {
            Visit(node as InNode);
            Instructions.Add(new NotInstruction());
        }

        /// <summary>
        ///     Performs "Var" specific operations.
        /// </summary>
        /// <param name="node">The "Not" node.</param>
        public virtual void Visit(VarNode node)
        {
            switch (node.Token.Value)
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
                    Instructions.Add(new LoadDateTimeVariableInstruction(v => v.ReferenceTime));
                    break;
                case "weekday":
                    Instructions.Add(new LoadNumericVariableInstruction(v => (int) v.DayOfWeek));
                    break;
            }
        }

        /// <summary>
        ///     Performs "Numeric" specific operations.
        /// </summary>
        /// <param name="node">The "Numeric" node.</param>
        public virtual void Visit(NumericNode node)
        {
            Instructions.Add(new PushNumericInstruction(node.Value));
        }

        /// <summary>
        ///     Performs "ArgList" specific operations.
        /// </summary>
        /// <param name="node">The "ArgList" node.</param>
        public virtual void Visit(ArgListNode node)
        {
            Instructions.Add(new LoadToRegister(Registers.B, node.Descendants.Length));
        }

        /// <summary>
        ///     Performs "Function" specific operations.
        /// </summary>
        /// <param name="node">The "Function" node.</param>
        public virtual void Visit(RawFunctionNode node)
        {
            Visit(node, out MethodInfo methodInfo);
        }

        /// <summary>
        ///     Call function and store it's value.
        /// </summary>
        /// <param name="node"></param>
        public void Visit(CallFunctionAndStoreValueNode node)
        {
            Visit(node, out MethodInfo methodInfo);

            switch (node.ReturnType.Name)
            {
                case nameof(DateTimeOffset):
                    Instructions.Add(new CopyAndStoreValue<DateTimeOffset>(methodInfo, x => x.Datetimes.Peek()));
                    break;
                default:
                    Instructions.Add(new CopyAndStoreValue<long>(methodInfo, x => x.Values.Peek()));
                    break;
            }
        }

        /// <summary>
        ///     Restores value of invoked function.
        /// </summary>
        /// <param name="node"></param>
        public void Visit(CachedFunctionNode node)
        {
            var argTypes = node.Descendants.Select(f => f.ReturnType).ToArray();
            var registeredFunction = _metadatas.GetMethod(node.Name, argTypes);

            switch (registeredFunction.ReturnType.Name)
            {
                case nameof(DateTimeOffset):
                    Instructions.Add(new RestoreValue(registeredFunction, (m, o) => m.Datetimes.Push((DateTimeOffset) o)));
                    break;
                default:
                    Instructions.Add(new RestoreValue(registeredFunction, (m, o) => m.Values.Push((long) o)));
                    break;
            }
        }

        /// <summary>
        ///     Performs "Equality" specific operations.
        /// </summary>
        /// <param name="node">The "Equality" node.</param>
        public virtual void Visit(EqualityNode node)
        {
            ExpressionGenerateInstructions<EqualityNumeric>(node);
        }

        /// <summary>
        ///     Performs "Diff" specific operations.
        /// </summary>
        /// <param name="node">The "Diff" node.</param>
        public virtual void Visit(DiffNode node)
        {
            ExpressionGenerateInstructions<DiffNumeric>(node);
        }

        /// <summary>
        ///     Performs "In" specific operations.
        /// </summary>
        /// <param name="node">The "In" node.</param>
        public virtual void Visit(InNode node)
        {
            switch (node.Left.ReturnType.GetTypeName())
            {
                case nameof(Int64):
                case nameof(Int32):
                case nameof(Int16):
                    ExpressionGenerateIn<InInstructionNumeric>(node);
                    break;
                case nameof(DateTimeOffset):
                    ExpressionGenerateIn<InInstructionDatetime>(node);
                    break;
            }
        }

        /// <summary>
        ///     Performs "And" specific operations.
        /// </summary>
        /// <param name="node">The "And" node.</param>
        public virtual void Visit(AndNode node)
        {
            ExpressionGenerateLeftToRightInstructions<AndInstruction>(node);
        }

        /// <summary>
        ///     Performs "RootScript" specific operations.
        /// </summary>
        /// <param name="node">The "RootScript" node.</param>
        public virtual void Visit(RootScriptNode node)
        {
            Instructions.Add(new Modify(_generateNext));
            Instructions.Add(new BreakInstruction());

            _startAt = new DateTimeOffset(_startAt.Year, _startAt.Month, _startAt.Day, _startAt.Hour, _startAt.Minute, _startAt.Second, TimeZoneInfo.Utc.BaseUtcOffset);
        }

        /// <summary>
        ///     Performs "Word" specific operations.
        /// </summary>
        /// <param name="node">The "Word" node.</param>
        public virtual void Visit(WordNode node)
        {
            //TO DO: Special language keyword should be moved to specialized method
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
                default:
                    Instructions.Add(new PushStringInstruction(node.Token.Value));
                    break;
            }
        }

        /// <summary>
        ///     Performs "Greater" specific operations.
        /// </summary>
        /// <param name="node">The "Numeric" node.</param>
        public virtual void Visit(GreaterNode node)
        {
            ExpressionGenerateInstructions<GreaterNumeric>(node);
        }

        /// <summary>
        ///     Performs "GreaterEqual" specific operations.
        /// </summary>
        /// <param name="node">The "Numeric" node.</param>
        public virtual void Visit(GreaterEqualNode node)
        {
            ExpressionGenerateInstructions<GreaterEqualNumeric>(node);
        }

        /// <summary>
        ///     Performs "Less" specific operations.
        /// </summary>
        /// <param name="node">The "Less" node.</param>
        public virtual void Visit(LessNode node)
        {
            ExpressionGenerateInstructions<LessNumeric>(node);
        }

        /// <summary>
        ///     Performs "LessEqual" specific operations.
        /// </summary>
        /// <param name="node">The "LessEqual" node.</param>
        public virtual void Visit(LessEqualNode node)
        {
            ExpressionGenerateInstructions<LessEqualNumeric>(node);
        }

        /// <summary>
        ///     Performs "Add" specific operations.
        /// </summary>
        /// <param name="node">The "Add" node.</param>
        public virtual void Visit(AddNode node)
        {
            ExpressionGenerateInstructions<AddNumericToNumeric>(node);
        }

        /// <summary>
        ///     Performs "Modulo" specific operations.
        /// </summary>
        /// <param name="node">The "Modulo" node.</param>
        public virtual void Visit(ModuloNode node)
        {
            ExpressionGenerateInstructions<ModuloNumericToNumeric>(node);
        }

        /// <summary>
        ///     Performs "Star" specific operations.
        /// </summary>
        /// <param name="node">The "Star" node.</param>
        public virtual void Visit(StarNode node)
        {
            Instructions.Add(new MultiplyNumerics());
        }

        /// <summary>
        ///     Performs "FSlash" specific operations.
        /// </summary>
        /// <param name="node">The "FSlash" node.</param>
        public virtual void Visit(FSlashNode node)
        {
            Instructions.Add(new DivideNumeric());
        }

        /// <summary>
        ///     Performs "Hyphen" specific operations.
        /// </summary>
        /// <param name="node">The "Hyphen" node.</param>
        public virtual void Visit(HyphenNode node)
        {
            Instructions.Add(new SubtractNumeric());
        }

        /// <summary>
        ///     Performs "Case" specific operations.
        /// </summary>
        /// <param name="node">The "Case" node.</param>
        public virtual void Visit(CaseNode node)
        {
        }

        /// <summary>
        ///     Performs "WhenThen" specific operations.
        /// </summary>
        /// <param name="node">The "WhenThen" node.</param>
        public virtual void Visit(WhenThenNode node)
        {
            _labels.Add($"when_{node.FullSpan.Start}{node.FullSpan.End}", Instructions.Count);
        }

        /// <summary>
        ///     Performs "When" specific operations.
        /// </summary>
        /// <param name="node">The "When" node.</param>
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
                Instructions.Add(
                    new JumpToLabelNotEqual($"when_{nextWhenNode.FullSpan.Start}{nextWhenNode.FullSpan.End}"));
            }
        }

        /// <summary>
        ///     Performs "Then" specific operations.
        /// </summary>
        /// <param name="node">The "Numeric" node.</param>
        public virtual void Visit(ThenNode node)
        {
            var elseSpan = node.Parent.Parent.Else.FullSpan;
            Instructions.Add(new JumpToLabel($"esac_{elseSpan.Start}{elseSpan.End}"));

            if (node.Parent.ArrayOrder == node.Parent.Parent.WhenThenExpressions.Count() - 1)
                _labels.Add($"else_{node.Parent.Parent.Else.FullSpan.Start}{node.Parent.Parent.Else.FullSpan.End}",
                    Instructions.Count);
        }

        /// <summary>
        ///     Performs "Else" specific operations.
        /// </summary>
        /// <param name="node">The "Else" node.</param>
        public virtual void Visit(ElseNode node)
        {
            _labels.Add($"esac_{node.Parent.Else.FullSpan.Start}{node.Parent.Else.FullSpan.End}", Instructions.Count);
        }

        /// <summary>
        ///     Performs "Not" specific operations.
        /// </summary>
        /// <param name="node">The "Not" node.</param>
        public void Visit(NotNode node)
        {
            Instructions.Add(new NotInstruction());
        }

        /// <summary>
        ///     Performs "Between" specific operations.
        /// </summary>
        /// <param name="node">The "Between" node.</param>
        public void Visit(BetweenNode node)
        {
            switch (node.Expression.ReturnType.Name)
            {
                case nameof(Boolean):
                case nameof(Int16):
                case nameof(Int32):
                case nameof(Int64):
                    Instructions.Add(new BetweenNumeric());
                    break;
                case nameof(DateTimeOffset):
                    Instructions.Add(new BetweenDatetime());
                    break;
            }
        }

        /// <summary>
        ///     Performs "Function" specific operations.
        /// </summary>
        /// <param name="node">The "Function" node.</param>
        /// <param name="foundedMethod">The founded .NET method.</param>
        private void Visit(RawFunctionNode node, out MethodInfo foundedMethod)
        {
            var argTypes = node.Descendants.Select(f => f.ReturnType).ToArray();
            var registeredMethod = _metadatas.GetMethod(node.Name, argTypes);
            foundedMethod = registeredMethod;
            var occurenceAmount = _functionOccurences[node.GeneralFunctionIdentifier()];
            var occurenceOrder = _functionOccurences[node.SpecificFunctionIdentifier()] - 1;
            Instructions.Add(new CallExternal(_callMethodContext, registeredMethod, argTypes.Length, _partOfDate, occurenceAmount, occurenceOrder));
        }

        private void ExpressionGenerateIn<TOperator>(InNode node)
            where TOperator : IRdlInstruction, new()
        {
            Instructions.Add(new TOperator());
        }

        private void ExpressionGenerateLeftToRightInstructions<TOperator>(BinaryNode node)
            where TOperator : IRdlInstruction, new()
        {
            Instructions.Add(new TOperator());
        }

        private void ExpressionGenerateInstructions<TNumericOp>(BinaryNode node)
            where TNumericOp : IRdlInstruction, new()
        {
            Instructions.Add(new TNumericOp());
        }

        #region Private variables

        private DateTimeOffset _startAt;
        private DateTimeOffset? _stopAt;
        private readonly Stack<List<IRdlInstruction>> _functions;
        private MemoryVariables _variables;
        private Modify.Fun _generateNext;
        private readonly Dictionary<string, int> _labels;
        private readonly RdlMetadata _metadatas;
        private readonly object _callMethodContext;
        private bool _hasWhereConditions;
        private PartOfDate _partOfDate;
        private readonly IReadOnlyDictionary<string, int> _functionOccurences;

        #endregion

        #region Private static variables

        private static readonly string NDateTime = Nullable.GetUnderlyingType(typeof(DateTimeOffset?)).Name;
        private static readonly string NInt64 = Nullable.GetUnderlyingType(typeof(long?)).Name;
        private static readonly string NBoolean = Nullable.GetUnderlyingType(typeof(bool?)).Name;

        #endregion

        #region Constructors

        /// <summary>
        ///     Instantiate code generator instance.
        /// </summary>
        /// <param name="metadatas">Metadata manager with functions registered.</param>
        /// <param name="callMethodContext">object that contains methods to invoke.</param>
        public CodeGenerator(RdlMetadata metadatas, object callMethodContext, IReadOnlyDictionary<string, int> functionOccurences)
            : this(metadatas, DateTimeOffset.UtcNow, callMethodContext, functionOccurences)
        {
        }

        /// <summary>
        ///     Instantiate code generator object with custom startAt
        /// </summary>
        /// <param name="metadatas">Metadata manager with functions registered.</param>
        /// <param name="startAt">Starting from parameter.</param>
        /// <param name="callMethodContext">Object that contains methods to invoke.</param>
        /// <param name="functionOccurences"></param>
        private CodeGenerator(RdlMetadata metadatas, DateTimeOffset startAt, object callMethodContext, IReadOnlyDictionary<string, int> functionOccurences)
        {
            _variables = new MemoryVariables();
            _functions = new Stack<List<IRdlInstruction>>();
            _functions.Push(new List<IRdlInstruction>());
            _stopAt = null;
            _labels = new Dictionary<string, int>();
            _metadatas = metadatas;
            _startAt = startAt;
            _callMethodContext = callMethodContext;
            _functionOccurences = functionOccurences;
        }

        #endregion
    }
}