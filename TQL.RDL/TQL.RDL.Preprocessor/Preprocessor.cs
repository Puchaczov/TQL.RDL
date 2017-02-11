using TQL.Common.Filters;
using TQL.Common.Pipeline;

namespace TQL.RDL.Preprocessor
{
    public class Preprocessor : Pipeline<string>
    {
        /// <summary>
        /// Initialize instance.
        /// </summary>
        public Preprocessor()
        {
            Register(new Trim());
        }
    }
}