﻿namespace TQL.RDL.Parser.Nodes
{
    public interface INodeVisitor
    {
        void Visit(FunctionNode node);
        void Visit(WordNode node);
        void Visit(WhereConditionsNode node);
        void Visit(StartAtNode node);
        void Visit(StopAtNode node);
        void Visit(RootScript node);
        void Visit(RepeatEveryNode node);
        void Visit(AndNode node);
        void Visit(OrNode node);
        void Visit(InNode node);
        void Visit(DateTimeNode node);
        void Visit(DiffNode node);
        void Visit(CommaNode node);
        void Visit(EqualityNode node);
        void Visit(NotNode node);
        void Visit(ArgListNode node);
        void Visit(VarNode node);
        void Visit(NumericNode node);
        void Visit(ConstantNode node);
        void Visit(GreaterNode node);
        void Visit(GreaterEqualNode node);
        void Visit(LessNode node);
        void Visit(LessEqualNode node);
    }
}