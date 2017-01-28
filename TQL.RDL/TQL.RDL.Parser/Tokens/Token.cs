using System;
using System.Diagnostics;
using TQL.Core.Tokens;

namespace RDL.Parser.Tokens
{
    [DebuggerDisplay("{Value} of type {TokenType},nq")]
    public class Token : GenericToken<StatementType>, IEquatable<Token>
    {
        public Token(string value, StatementType type, TextSpan span) 
            : base(value, type, span)
        { }

        public bool Equals(Token other) => other.TokenType == TokenType && other.Value == Value;

        public override GenericToken<StatementType> Clone() => new Token(Value, TokenType, Span);

        public override bool Equals(object obj)
        {
            var token = obj as Token;
            if(token == null)
            {
                return false;
            }
            if(ReferenceEquals(obj, this))
            {
                return true;
            }
            return TokenType == token.TokenType && Value == token.Value;
        }

        public override int GetHashCode() => 17 * TokenType.GetHashCode() + Value.GetHashCode();

        public override string ToString() => Value;
    }
}
