﻿namespace TQL.RDL.Parser.Tokens
{
    public enum SyntaxType : short
    {
        Word,
        Numeric,
        LeftParenthesis,
        RightParenthesis,
        None,
        EndOfFile,
        Diff,
        And,
        Or,
        Not,
        Is,
        Repeat,
        Where,
        In,
        Every,
        Plus,
        Star,
        FSlash,
        Hyphen,
        Mod,
        Comma,
        WhiteSpace,
        Equality,
        StartAt,
        StopAt,
        Function,
        VarArg,
        Var,
        Greater,
        GreaterEqual,
        Less,
        LessEqual,
        NotIn
    }
}
