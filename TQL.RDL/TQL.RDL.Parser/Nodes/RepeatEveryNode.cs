using System;
using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class RepeatEveryNode : RdlSyntaxNode
    {
        private readonly Token _repeatEvery;
        private readonly Token _timeToken;

        public RepeatEveryNode(Token repeatEvery, Token timeToken)
        {
            _repeatEvery = repeatEvery;
            _timeToken = timeToken;
        }

        public override RdlSyntaxNode[] Descendants => new RdlSyntaxNode[0];

        public override TextSpan FullSpan
            => new TextSpan(_repeatEvery.Span.Start, _timeToken.Span.End - _repeatEvery.Span.Start);

        public override bool IsLeaf => true;

        public override Token Token => _timeToken;

        public PartOfDate DatePart
        {
            get
            {
                switch (Token.Value)
                {
                    case "seconds":
                        return PartOfDate.Seconds;
                    case "hours":
                        return PartOfDate.Hours;
                    case "minutes":
                        return PartOfDate.Minutes;
                    case "days":
                        return PartOfDate.DaysOfMonth;
                    case "months":
                        return PartOfDate.Months;
                    case "years":
                        return PartOfDate.Years;
                }
                return PartOfDate.Unknown;
            }
        }

        public virtual int Value => 1;

        public override Type ReturnType => typeof(long);

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public override string ToString() => $"repeat every {Value}";
    }
}