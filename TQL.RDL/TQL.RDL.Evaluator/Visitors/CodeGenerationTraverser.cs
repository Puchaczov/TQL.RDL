using System;
using System.Collections.Generic;
using TQL.RDL.Evaluator.Enumerators;
using TQL.RDL.Parser.Nodes;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Evaluator.Visitors
{
    public class CodeGenerationTraverser : INodeVisitor
    {
        private INodeVisitor _codeGenerationVisitor;

        public CodeGenerationTraverser(INodeVisitor codeGenerationVisitor)
        {
            _codeGenerationVisitor = codeGenerationVisitor;
        }

        public virtual void Visit(WhereConditionsNode node) {
            foreach(var item in node.Descendants)
            {
                item.Accept(this);
            }
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(StopAtNode node) {
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(RepeatEveryNode node) {
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(OrNode node) {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(DateTimeNode node)
        {
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(EqualityNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(ArgListNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(NumericNode node)
        {
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(GreaterEqualNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(LessEqualNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(AddNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(ModuloNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(FSlashNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(ThenNode node)
        {
            node.Descendant.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(CaseNode node)
        {
            node.Accept(_codeGenerationVisitor);
            foreach (var item in node.Descendants)
                item.Accept(this);
        }

        public virtual void Visit(WhenThenNode node)
        {
            node.Accept(_codeGenerationVisitor);
            foreach (var item in node.Descendants)
                item.Accept(this);
        }

        public virtual void Visit(ElseNode node)
        {
            node.Descendant.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(WhenNode node)
        {
            node.Descendant.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(StarNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(HyphenNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(NumericConsequentRepeatEveryNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(LessNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(GreaterNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(VarNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(NotInNode node)
        {
            node.Right.Accept(this);
            node.Left.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(DiffNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(InNode node)
        {
            node.Right.Accept(this);
            node.Left.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(AndNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(RootScriptNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(StartAtNode node)
        {
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(WordNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }

        public virtual void Visit(FunctionNode node)
        {
            foreach (var item in node.Descendants)
                item.Accept(this);
            node.Accept(_codeGenerationVisitor);
        }
    }
}
