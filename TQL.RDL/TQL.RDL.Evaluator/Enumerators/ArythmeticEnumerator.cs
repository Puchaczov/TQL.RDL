using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Enumerators
{
    public class ArythmeticEnumerator : EnumeratorBase<Stack<VisitationState>>, IEnumerator<RdlSyntaxNode>
    {
        public ArythmeticEnumerator(RdlSyntaxNode node)
            : base(node)
        {
            this.stack.Push(new VisitationState(this.root));
        }

        public override void Dispose() { }

        public override bool MoveNext()
        {
            while (stack.Count > 0)
            {
                var n = stack.Pop();

                //jest lisciem i nie byl jeszcze odwiedzany
                if (n.Node.IsLeaf && !n.WasVisitedOnce)
                {
                    this.Current = n.Node;
                    return true;
                }
                //nie jest lisciem i wszystkie dzieci zostaly odwiedzone
                else if (!n.Node.IsLeaf && n.ToVisitDescendantIndex == n.Node.Descendants.Count())
                {
                    this.Current = n.Node;
                    return true;
                }
                else
                {
                    if (!n.WasVisitedOnce)
                    {
                        n.WasVisitedOnce = true;
                        stack.Push(n);
                    }
                    //jezeli zostalo jeszcze jakikolwiek dziecko do odwiedzenia
                    else if (n.ToVisitDescendantIndex < n.Node.Descendants.Count())
                    {
                        stack.Push(n);
                    }

                    for (int i = n.ToVisitDescendantIndex, j = n.Node.Descendants.Count(); i < j; ++i)
                    {
                        n.ToVisitDescendantIndex = i + 1;
                        stack.Push(new VisitationState(n.Node.Descendants[i]));
                        if (n.Node.Descendants[i].IsLeaf)
                        {
                            stack.Peek().WasVisitedOnce = true;
                            Current = n.Node.Descendants[i];
                            return true;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return false;
        }

        public override void Reset()
        {
            stack.Clear();
            stack.Push(new VisitationState(root));
        }
    }
}
