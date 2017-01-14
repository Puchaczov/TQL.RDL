using System;
using System.Collections.Generic;
using TQL.RDL.Evaluator.Enumerators;
using TQL.RDL.Parser.Nodes;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Evaluator.Visitors
{
    public sealed class CodeGenerationTraverser : INodeVisitor
    {
        private readonly INodeVisitor _codeGenerationVisitor;

        public CodeGenerationTraverser(INodeVisitor codeGenerationVisitor)
        {
            if (codeGenerationVisitor == null) throw new ArgumentNullException(nameof(codeGenerationVisitor));

            _codeGenerationVisitor = codeGenerationVisitor;
        }

        public void Visit(WhereConditionsNode node) {
            foreach(var item in node.Descendants)
            {
                item.Accept(this);
            }
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(StopAtNode node) {
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(RepeatEveryNode node) {
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(OrNode node) {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(DateTimeNode node)
        {
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(EqualityNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(ArgListNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(NumericNode node)
        {
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(GreaterEqualNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(LessEqualNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(AddNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(ModuloNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(FSlashNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(ThenNode node)
        {
            node.Descendant.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(CaseNode node)
        {
            node.Accept(_codeGenerationVisitor);
            foreach (var item in node.Descendants)
                item.Accept(this);
        }

        public void Visit(WhenThenNode node)
        {
            node.Accept(_codeGenerationVisitor);
            foreach (var item in node.Descendants)
                item.Accept(this);
        }

        public void Visit(ElseNode node)
        {
            node.Descendant.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(WhenNode node)
        {
            node.Descendant.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(StarNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(HyphenNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(NumericConsequentRepeatEveryNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(LessNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(GreaterNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(VarNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(NotInNode node)
        {
            node.Right.Accept(this);
            node.Left.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(DiffNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(InNode node)
        {
            node.Right.Accept(this);
            node.Left.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(AndNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(RootScriptNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(StartAtNode node)
        {
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(WordNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public void Visit(FunctionNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }
    }
}
