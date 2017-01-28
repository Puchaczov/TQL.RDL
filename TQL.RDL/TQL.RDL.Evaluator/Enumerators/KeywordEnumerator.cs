using System.Collections.Generic;
using RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Enumerators
{
    public class KeywordEnumerator : EnumeratorBase<Stack<VisitationState>>, IEnumerator<RdlSyntaxNode>
    {
        /// <summary>
        /// Initialize object.
        /// </summary>
        /// <param name="root">Root element where enumeration will start from.</param>
        public KeywordEnumerator(RdlSyntaxNode root) 
            : base(root)
        {
            Stack.Push(new VisitationState(root));
        }

        /// <summary>
        /// Dispose enumerator.
        /// </summary>
        public override void Dispose() { }

        /// <summary>
        /// Causes enumerator to move to next element.
        /// </summary>
        /// <returns>Move to the next element passed.</returns>
        public override bool MoveNext()
        {
            while(Stack.Count > 0)
            {
                var node = Stack.Pop();

                for(var i = node.Node.Descendants.Length - 1; i >= 0; --i)
                {
                    Stack.Push(new VisitationState(node.Node.Descendants[i]));
                }

                Current = node.Node;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reset enumerator to start from root element.
        /// </summary>
        public override void Reset()
        {
            Stack.Clear();
            Stack.Push(new VisitationState(Root));
        }
    }
}
