﻿using TQL.Core.Tokens;

namespace RDL.Parser.Tokens
{
    public class NumericToken : Token
    {
        public const string TokenText = "numeric";

        public NumericToken(string value, TextSpan span)
            : base(value, StatementType.Numeric, span)
        {
        }
    }
}