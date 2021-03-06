﻿using System;
using System.Collections.Generic;
using TQL.RDL.Evaluator.Instructions;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Visitors
{
    public sealed class DebuggerSymbolGenerator : CodeGenerator
    {
        public DebuggerSymbolGenerator(RdlMetadata gm, object callMethodContext, IReadOnlyDictionary<string, int> functionOccurences)
            : base(gm, callMethodContext, functionOccurences)
        {
        }

        public override void Visit(AddNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(OrNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(AndNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(DiffNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(GreaterNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(LessEqualNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(CaseNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(ElseNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(EqualityNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(RawFunctionNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(FSlashNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(GreaterEqualNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(WhereConditionsNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(HyphenNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(ModuloNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));

        public override void Visit(NumericConsequentRepeatEveryNode node)
            => ProduceDebuggerInstructions(node, n => base.Visit(n));

        public override void Visit(ThenNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(RootScriptNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(WhenNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(RepeatEveryNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(InNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(WhenThenNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(NotInNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));
        public override void Visit(LessNode node) => ProduceDebuggerInstructions(node, n => base.Visit(n));

        private void ProduceDebuggerInstructions<TNode>(TNode node, Action<TNode> visit)
            where TNode : RdlSyntaxNode
        {
            Instructions.Add(new DebuggerTrap(node, DebuggerTrap.ExpressionState.Before, (n, m) => { }, (n, m) => { }));
            visit?.Invoke(node);
            Instructions.Add(new DebuggerTrap(node, DebuggerTrap.ExpressionState.After, (n, m) => { }, (n, m) => { }));
        }
    }
}