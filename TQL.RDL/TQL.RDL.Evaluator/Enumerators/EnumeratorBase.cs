using System.Collections;
using System.Collections.Generic;
using RDL.Parser.Nodes;

namespace TQL.RDL.Evaluator.Enumerators
{
    public abstract class EnumeratorBase<TStoreType> : IEnumerator<RdlSyntaxNode> where TStoreType : new()
    {
        /// <summary>
        /// Initialize object.
        /// </summary>
        /// <param name="root">Root node where enumeration will start from.</param>
        public EnumeratorBase(RdlSyntaxNode root)
        {
            Root = root;
        }

        /// <summary>
        /// Get element that is enumerated.
        /// </summary>
        public RdlSyntaxNode Current { get; protected set; }

        /// <summary>
        /// Get element that is enumereted.
        /// </summary>
        object IEnumerator.Current => Current;

        /// <summary>
        /// Dispose element
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Causes enumerator to move to next element.
        /// </summary>
        /// <returns>Move to the next element passed.</returns>
        public abstract bool MoveNext();

        /// <summary>
        /// Reset enumerator to start from root element.
        /// </summary>
        public abstract void Reset();

        #region Protected variables

        protected readonly TStoreType Stack = new TStoreType();
        protected readonly RdlSyntaxNode Root;

        #endregion
    }
}
