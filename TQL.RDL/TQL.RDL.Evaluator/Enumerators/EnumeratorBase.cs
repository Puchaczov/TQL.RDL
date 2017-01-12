using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Enumerators
{
    public abstract class EnumeratorBase<TStoreType> : IEnumerator<RdlSyntaxNode> where TStoreType : new()
    {
        protected TStoreType Stack = new TStoreType();
        protected readonly RdlSyntaxNode Root;

        public EnumeratorBase(RdlSyntaxNode root)
        {
            Root = root;
        }

        public RdlSyntaxNode Current { get; protected set; }

        object IEnumerator.Current => Current;

        public abstract void Dispose();

        public abstract bool MoveNext();

        public abstract void Reset();
    }
}
