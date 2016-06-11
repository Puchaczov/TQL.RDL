using System;
using TQL.Common.Pipeline;

namespace TQL.RDL
{
    internal class MakeKeywordsToLower 
        : IFilter<string>
    {
        public string Execute(string input) => input;

        public void Register(IFilter<string> filter)
        {
            throw new NotImplementedException();
        }
    }
}