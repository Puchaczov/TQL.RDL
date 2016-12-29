using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TQL.RDL.Evaluator.Enumerators;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator
{
    public abstract class AnalyzerBase : INodeVisitor
    {
        public abstract void Visit(WhereConditionsNode node);

        public abstract void Visit(StopAtNode node);

        public abstract void Visit(RepeatEveryNode node);

        public abstract void Visit(OrNode node);

        public abstract void Visit(DateTimeNode node);

        public abstract void Visit(EqualityNode node);

        public abstract void Visit(ArgListNode node);

        public abstract void Visit(NumericNode node);

        public abstract void Visit(GreaterEqualNode node);

        public abstract void Visit(LessEqualNode node);

        public abstract void Visit(AddNode node);

        public abstract void Visit(ModuloNode node);

        public abstract void Visit(FSlashNode node);

        public abstract void Visit(ThenNode node);

        public abstract void Visit(CaseNode node);

        public abstract void Visit(WhenThenNode node);

        public abstract void Visit(ElseNode node);

        public abstract void Visit(WhenNode node);

        public abstract void Visit(StarNode node);

        public abstract void Visit(HyphenNode node);

        public abstract void Visit(NumericConsequentRepeatEveryNode node);

        public abstract void Visit(LessNode node);

        public abstract void Visit(GreaterNode node);

        public abstract void Visit(VarNode node);

        public abstract void Visit(NotInNode node);

        public abstract void Visit(DiffNode node);

        public abstract void Visit(InNode node);

        public abstract void Visit(AndNode node);

        public abstract void Visit(RootScriptNode node);

        public abstract void Visit(StartAtNode node);

        public abstract void Visit(WordNode node);

        public abstract void Visit(FunctionNode node);
    }
}
