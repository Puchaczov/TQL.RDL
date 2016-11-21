using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator
{
    public abstract class RDLAnalyzerBase : INodeVisitor
    {
        [DebuggerDisplay("{Node.ToString(),nq}")]
        private class VisitationState
        {
            public RdlSyntaxNode Node { get; }
            public int ToVisitDescendantIndex { get; set; }
            public bool WasVisitedOnce { get; set; }

            public VisitationState(RdlSyntaxNode node)
            {
                this.Node = node;
                ToVisitDescendantIndex = node.Descendants.Count() - 1;
                WasVisitedOnce = false;
            }

            public void Accept(RDLAnalyzerBase analyzer)
            {
                Node.Accept(analyzer);
                WasVisitedOnce = true;
            }
        }

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

        public virtual void Visit(RootScriptNode node)
        {
            var stack = new Stack<VisitationState>();

            foreach (var i in node.Descendants.Reverse())
                stack.Push(new VisitationState(i));

            while (stack.Count > 0)
            {
                var n = stack.Pop();
                
                if (n.Node.IsLeaf && !n.WasVisitedOnce)
                {
                    n.Accept(this);
                }
                else if(!n.Node.IsLeaf && n.ToVisitDescendantIndex == -1)
                {
                    n.Accept(this);
                }
                else
                {
                    if(!n.WasVisitedOnce)
                    {
                        n.WasVisitedOnce = true;
                        stack.Push(n);
                    }
                    else if(n.ToVisitDescendantIndex >= 0)
                    {
                        stack.Push(n);
                    }

                    for (int i = n.ToVisitDescendantIndex; i >= 0; --i)
                    {
                        n.ToVisitDescendantIndex = i - 1;
                        stack.Push(new VisitationState(n.Node.Descendants[i]));
                        if (n.Node.Descendants[i].IsLeaf)
                        {
                            stack.Peek().WasVisitedOnce = true;
                            n.Node.Descendants[i].Accept(this);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        public abstract void Visit(StartAtNode node);

        public abstract void Visit(WordNode node);

        public abstract void Visit(FunctionNode node);
    }
}
