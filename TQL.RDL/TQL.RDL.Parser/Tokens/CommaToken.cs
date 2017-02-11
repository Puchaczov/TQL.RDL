﻿using TQL.Core.Tokens;

namespace RDL.Parser.Tokens
{
    public class CommaToken : Token
    {
        public const string TokenText = ",";

        public CommaToken(TextSpan span)
            : base(TokenText, StatementType.Comma, span)
        {
        }
    }
}