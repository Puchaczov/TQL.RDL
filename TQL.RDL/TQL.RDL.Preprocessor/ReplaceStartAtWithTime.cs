using System;
using System.Text.RegularExpressions;
using TQL.Common.Filters.Pipeline;

namespace TQL.RDL.Preprocessor
{
    public class ReplaceStartAtWithTime : FilterBase<string>
    {
        private DateTimeOffset _time;
        private string[] _formats;
        private readonly string _quotedDateRegex;

        public ReplaceStartAtWithTime(DateTimeOffset time, string[] formats, string quotedDateRegex)
        {
            _time = time;
            _formats = formats;
            _quotedDateRegex = quotedDateRegex;
        }

        protected override string Process(string input)
        {
            string foundedFormat = _formats[0];
            var match = Regex.Match(input, _quotedDateRegex);
            if(!match.Success)
            {
                return input;
            }

            var quotedDate = match.Groups[2].Value;
            quotedDate = quotedDate.Replace("dd", _time.Day.ToString().PadLeft(2, '0'));
            quotedDate = quotedDate.Replace("d", _time.Day.ToString());
            quotedDate = quotedDate.Replace("MM", _time.Month.ToString().PadLeft(2, '0'));
            quotedDate = quotedDate.Replace("M", _time.Month.ToString());
            quotedDate = quotedDate.Replace("yyyy", _time.Year.ToString().PadLeft(4, '0'));

            quotedDate = quotedDate.Replace("HH", _time.Hour.ToString().PadLeft(2, '0'));
            quotedDate = quotedDate.Replace("hh", _time.Hour.ToString().PadLeft(2, '0'));
            quotedDate = quotedDate.Replace("h", _time.Hour.ToString());
            quotedDate = quotedDate.Replace("H", _time.Hour.ToString());
            quotedDate = quotedDate.Replace("mm", _time.Hour.ToString().PadLeft(2, '0'));
            quotedDate = quotedDate.Replace("m", _time.Hour.ToString());
            quotedDate = quotedDate.Replace("ss", _time.Hour.ToString().PadLeft(2, '0'));
            quotedDate = quotedDate.Replace("s", _time.Hour.ToString());

            return input.Replace(match.Groups[2].Value, quotedDate);
        }
    }
}