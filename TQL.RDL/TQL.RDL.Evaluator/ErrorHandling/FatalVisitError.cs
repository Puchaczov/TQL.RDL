using System;

namespace TQL.RDL.Evaluator.ErrorHandling
{
    public class FatalVisitError : VisitationMessage
    {
        private readonly Exception _exc;

        /// <summary>
        ///     Initialize object.
        /// </summary>
        /// <param name="exc">Catched exception.</param>
        public FatalVisitError(Exception exc)
            : base(null, exc.Message)
        {
            _exc = exc;
        }

        /// <summary>
        ///     Group information identifier (FatalError)
        /// </summary>
        public override Codes Code => Codes.C01;

        /// <summary>
        ///     Message level of information (Error)
        /// </summary>
        public override MessageLevel Level => MessageLevel.Error;

        /// <summary>
        ///     Stringify object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Code}/{Level}/{Message}";
    }
}