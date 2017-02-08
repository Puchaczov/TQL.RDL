using System;

namespace TQL.RDL.Evaluator.Attributes
{
    public abstract class InjectTypeAttribute : Attribute
    {
        /// <summary>
        /// Initialize object.
        /// </summary>
        internal InjectTypeAttribute() { }

        /// <summary>
        /// Gets the type have to be injected when dynamic invocation performed.
        /// </summary>
        public abstract Type InjectType { get; }
    }
}
