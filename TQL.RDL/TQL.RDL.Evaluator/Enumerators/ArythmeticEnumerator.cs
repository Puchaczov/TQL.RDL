﻿using System.Collections.Generic;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Enumerators
{
    public class ArythmeticEnumerator : EnumeratorBase<Stack<VisitationState>>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Create instance of tree enumerator
        /// </summary>
        /// <param name="node">Abstract Syntax Tree</param>
        public ArythmeticEnumerator(RdlSyntaxNode node)
            : base(node)
        {
            Stack.Push(new VisitationState(Root));
        }

        /// <summary>
        ///     Dispose unused elements
        /// </summary>
        public override void Dispose()
        {
        }

        /// <summary>
        ///     Determine if next element to visit exist
        /// </summary>
        /// <returns>Next node to visit exist</returns>
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
                if (!n.Node.IsLeaf && n.ToVisitDescendantIndex == n.Node.Descendants.Length)
                {
                    Current = n.Node;
                    return true;
                }
                if (!n.WasVisitedOnce)
                {
                    n.WasVisitedOnce = true;
                    Stack.Push(n);
                }
                //jezeli zostalo jeszcze jakikolwiek dziecko do odwiedzenia
                else if (n.ToVisitDescendantIndex < n.Node.Descendants.Length)
                {
                    Stack.Push(n);
                }

                for (int i = n.ToVisitDescendantIndex, j = n.Node.Descendants.Length; i < j; ++i)
                {
                    n.ToVisitDescendantIndex = i + 1;
                    Stack.Push(new VisitationState(n.Node.Descendants[i]));
                    if (n.Node.Descendants[i].IsLeaf)
                    {
                        Stack.Peek().WasVisitedOnce = true;
                        Current = n.Node.Descendants[i];
                        return true;
                    }
                    break;
                }
            }
            return false;
        }

        /// <summary>
        ///     Reset enumerator to start from root
        /// </summary>
        public override void Reset()
        {
            Stack.Clear();
            Stack.Push(new VisitationState(Root));
        }
    }
}