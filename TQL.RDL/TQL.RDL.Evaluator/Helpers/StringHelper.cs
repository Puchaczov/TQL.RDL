using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TQL.RDL.Evaluator.Helpers
{
    public static class StringHelper
    {
        private static readonly Dictionary<string, int> KeywordMapper;

        static StringHelper()
        {
            KeywordMapper = new Dictionary<string, int>()
            {
                { "monday", (int)DayOfWeek.Monday },
                { "tuesday", (int)DayOfWeek.Tuesday },
                { "wednesday", (int)DayOfWeek.Wednesday },
                { "thursday", (int)DayOfWeek.Thursday },
                { "friday", (int)DayOfWeek.Friday },
                { "saturday", (int)DayOfWeek.Saturday },
                { "sunday", (int)DayOfWeek.Sunday },
                { "january", 100 },
                { "february", 101 },
                { "march", 102 },
                { "april", 103 },
                { "may", 104 },
                { "june", 105 },
                { "july", 106 },
                { "august", 107 },
                { "september", 108 },
                { "november", 109 },
                { "october", 110 },
                { "december", 111 }
            };
        }

        /// <summary>
        /// Determine if passed word is language keyword.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <returns>True is word is language keyword, else false.</returns>
        public static bool IsKeyword(this string keyword)
        {
            int result;
            return TryGetKeyword(keyword, out result);
        }

        /// <summary>
        /// Checks if word is language keyword and get appropiate mapped numeric.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <param name="result">Number mapped to passed keyword.</param>
        /// <returns>True is word is language keyword, else false.</returns>
        public static bool TryGetKeyword(this string keyword, out int result)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                result = -1;
                return false;
            }

            keyword = keyword.ToLowerInvariant();
            if (KeywordMapper.ContainsKey(keyword))
            {
                result = KeywordMapper[keyword];
                return true;
            }

            result = -1;
            return false;
        }
    }
}
