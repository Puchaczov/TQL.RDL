using System;
using System.Diagnostics;
using TQL.Core.Tokens;

namespace TQL.RDL.Parser.Tokens
{
    [DebuggerDisplay("{Value} of type {TokenType},nq")]
    public class Token : GenericToken<SyntaxType>, IEquatable<Token>
    {
        public Token(string value, SyntaxType type, TextSpan span) 
            : base(value, type, span)
        { }

        public override GenericToken<SyntaxType> Clone() => new Token(Value, TokenType, Span);

        public bool Equals(Token other) => other.TokenType == this.TokenType && other.Value == this.Value;

        public override bool Equals(object obj)
        {
            Token token = obj as Token;
            if(token == null)
            {
                return false;
            }
            if(Object.ReferenceEquals(obj, this))
            {
                return true;
            }
            return this.TokenType == token.TokenType && this.Value == token.Value;
        }

        public override int GetHashCode() => 17 * TokenType.GetHashCode() + Value.GetHashCode();
    }
}
