using System;

namespace TQL.RDL.Evaluator.Attributes
{
    public abstract class InjectTypeAttribute : Attribute
    {
        internal InjectTypeAttribute() { }

        public abstract Type InjectType { get; }
    }
}
