using System;
using TQL.RDL.Evaluator.Instructions;
using TQL.RDL.Parser;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator
{
    public sealed class RdlDebuggerSymbolGenerator : RdlCodeGenerator
    {
        public void Visit(AddNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(OrNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(AndNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(DiffNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(GreaterNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(LessEqualNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(CaseNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(ElseNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(EqualityNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(FunctionNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(FSlashNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(GreaterEqualNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(WhereConditionsNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(HyphenNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(ModuloNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(NumericConsequentRepeatEveryNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(ThenNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(RootScriptNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(WhenNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(RepeatEveryNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(InNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(WhenThenNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(NotInNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));
        public void Visit(LessNode node) => ProduceDebuggerInstructions(node, (n) => base.Visit(n));

        public RdlDebuggerSymbolGenerator(RdlMetadata gm)
            : base(gm)
        { }

        private void ProduceDebuggerInstructions<TNode>(TNode node, Action<TNode> visit)
            where TNode : RdlSyntaxNode
        {
            Instructions.Add(new DebuggerTrap(node, DebuggerTrap.ExpressionState.Before, (n, m) => { }, (n, m) => { }));
            visit(node);
            Instructions.Add(new DebuggerTrap(node, DebuggerTrap.ExpressionState.After, (n, m) => { }, (n, m) => { }));
        }
    }
}
