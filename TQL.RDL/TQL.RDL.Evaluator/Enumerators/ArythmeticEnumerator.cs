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
            Stack.Push(new VisitationState(Root));
        }

        public override void Dispose() { }

        public override bool MoveNext()
        {
            while (Stack.Count > 0)
            {
                var n = Stack.Pop();

                //jest lisciem i nie byl jeszcze odwiedzany
                if (n.Node.IsLeaf && !n.WasVisitedOnce)
                {
                    Current = n.Node;
                    return true;
                }
                //nie jest lisciem i wszystkie dzieci zostaly odwiedzone
                else if (!n.Node.IsLeaf && n.ToVisitDescendantIndex == n.Node.Descendants.Count())
                {
                    Current = n.Node;
                    return true;
                }
                else
                {
                    if (!n.WasVisitedOnce)
                    {
                        n.WasVisitedOnce = true;
                        Stack.Push(n);
                    }
                    //jezeli zostalo jeszcze jakikolwiek dziecko do odwiedzenia
                    else if (n.ToVisitDescendantIndex < n.Node.Descendants.Count())
                    {
                        Stack.Push(n);
                    }

                    for (int i = n.ToVisitDescendantIndex, j = n.Node.Descendants.Count(); i < j; ++i)
                    {
                        n.ToVisitDescendantIndex = i + 1;
                        Stack.Push(new VisitationState(n.Node.Descendants[i]));
                        if (n.Node.Descendants[i].IsLeaf)
                        {
                            Stack.Peek().WasVisitedOnce = true;
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
            Stack.Clear();
            Stack.Push(new VisitationState(Root));
        }
    }
}
