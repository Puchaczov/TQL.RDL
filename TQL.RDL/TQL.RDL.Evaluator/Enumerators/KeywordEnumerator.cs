using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Enumerators
{
    public class KeywordEnumerator : EnumeratorBase<Stack<VisitationState>>, IEnumerator<RdlSyntaxNode>
    {
        public KeywordEnumerator(RdlSyntaxNode root) 
            : base(root)
        {
            stack.Push(new VisitationState(root));
        }

        public override void Dispose() { }

        public override bool MoveNext()
        {
            while(stack.Count > 0)
            {
                var node = stack.Pop();

                for(int i = node.Node.Descendants.Length - 1; i >= 0; --i)
                {
                    stack.Push(new VisitationState(node.Node.Descendants[i]));
                }

                Current = node.Node;
                return true;
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
