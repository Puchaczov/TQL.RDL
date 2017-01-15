using RDL.Parser.Nodes;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Visitors
{
    public abstract class AnalyzerBase : INodeVisitor
    {
        /// <summary>
        /// Visit WhereCondition node.
        /// </summary>
        /// <param name="node">Where node of AST.</param>
        public abstract void Visit(WhereConditionsNode node);

        /// <summary>
        /// Visit StopAt node.
        /// </summary>
        /// <param name="node">StopAt node of AST.</param>
        public abstract void Visit(StopAtNode node);

        /// <summary>
        /// Visit RepeatEvery node.
        /// </summary>
        /// <param name="node">Where node of AST</param>
        public abstract void Visit(RepeatEveryNode node);

        /// <summary>
        /// Visit Or node.
        /// </summary>
        /// <param name="node">Or node of AST</param>
        public abstract void Visit(OrNode node);

        /// <summary>
        /// Visit DateTime node.
        /// </summary>
        /// <param name="node">DateTime node of AST</param>
        public abstract void Visit(DateTimeNode node);

        /// <summary>
        /// Visit Equality node.
        /// </summary>
        /// <param name="node">Equality node of AST</param>
        public abstract void Visit(EqualityNode node);

        /// <summary>
        /// Visit RepeatEvery node.
        /// </summary>
        /// <param name="node">Where node of AST</param>
        public abstract void Visit(ArgListNode node);

        /// <summary>
        /// Visit Numeric node.
        /// </summary>
        /// <param name="node">Numeric node of AST</param>
        public abstract void Visit(NumericNode node);

        /// <summary>
        /// Visit GreaterEqual node.
        /// </summary>
        /// <param name="node">GreaterEqual node of AST</param>
        public abstract void Visit(GreaterEqualNode node);

        /// <summary>
        /// Visit LessEqual node.
        /// </summary>
        /// <param name="node">LessEqual node of AST</param>
        public abstract void Visit(LessEqualNode node);

        /// <summary>
        /// Visit Add node.
        /// </summary>
        /// <param name="node">Add node of AST</param>
        public abstract void Visit(AddNode node);

        /// <summary>
        /// Visit Modulo node.
        /// </summary>
        /// <param name="node">Modulo node of AST</param>
        public abstract void Visit(ModuloNode node);

        /// <summary>
        /// Visit FSlash node.
        /// </summary>
        /// <param name="node">FSlash node of AST</param>
        public abstract void Visit(FSlashNode node);

        /// <summary>
        /// Visit Then node.
        /// </summary>
        /// <param name="node">Then node of AST</param>
        public abstract void Visit(ThenNode node);

        /// <summary>
        /// Visit Case node.
        /// </summary>
        /// <param name="node">Case node of AST</param>
        public abstract void Visit(CaseNode node);

        /// <summary>
        /// Visit WhenThen node.
        /// </summary>
        /// <param name="node">WhenThen node of AST</param>
        public abstract void Visit(WhenThenNode node);

        /// <summary>
        /// Visit Else node.
        /// </summary>
        /// <param name="node">Else node of AST</param>
        public abstract void Visit(ElseNode node);

        /// <summary>
        /// Visit When node.
        /// </summary>
        /// <param name="node">When node of AST</param>
        public abstract void Visit(WhenNode node);

        /// <summary>
        /// Visit Star node.
        /// </summary>
        /// <param name="node">Star node of AST</param>
        public abstract void Visit(StarNode node);

        /// <summary>
        /// Visit RepeatEvery node.
        /// </summary>
        /// <param name="node">Where node of AST</param>
        public abstract void Visit(HyphenNode node);

        /// <summary>
        /// Visit NumericConsequentRepeatEvery node.
        /// </summary>
        /// <param name="node">NumericConsequentRepeatEvery node of AST</param>
        public abstract void Visit(NumericConsequentRepeatEveryNode node);

        /// <summary>
        /// Visit Less node.
        /// </summary>
        /// <param name="node">Less node of AST</param>
        public abstract void Visit(LessNode node);

        /// <summary>
        /// Visit Greater node.
        /// </summary>
        /// <param name="node">Greater node of AST</param>
        public abstract void Visit(GreaterNode node);

        /// <summary>
        /// Visit Var node.
        /// </summary>
        /// <param name="node">Var node of AST</param>
        public abstract void Visit(VarNode node);

        /// <summary>
        /// Visit NotIn node.
        /// </summary>
        /// <param name="node">NotIn node of AST</param>
        public abstract void Visit(NotInNode node);

        /// <summary>
        /// Visit Diff node.
        /// </summary>
        /// <param name="node">Diff node of AST</param>
        public abstract void Visit(DiffNode node);

        /// <summary>
        /// Visit In node.
        /// </summary>
        /// <param name="node">In node of AST</param>
        public abstract void Visit(InNode node);

        /// <summary>
        /// Visit And node.
        /// </summary>
        /// <param name="node">And node of AST</param>
        public abstract void Visit(AndNode node);

        /// <summary>
        /// Visit RootScript node.
        /// </summary>
        /// <param name="node">RootScript node of AST</param>
        public abstract void Visit(RootScriptNode node);

        /// <summary>
        /// Visit StartAt node.
        /// </summary>
        /// <param name="node">StartAt node of AST</param>
        public abstract void Visit(StartAtNode node);

        /// <summary>
        /// Visit Word node.
        /// </summary>
        /// <param name="node">Word node of AST</param>
        public abstract void Visit(WordNode node);

        /// <summary>
        /// Visit Function node.
        /// </summary>
        /// <param name="node">Fuction node of AST</param>
        public abstract void Visit(FunctionNode node);
    }
}
