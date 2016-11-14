using System;
using TQL.Core.Tokens;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Parser.Nodes
{
    public class RepeatEveryNode : RdlSyntaxNode
    {
        protected Token repeatEvery;
        protected Token timeToken;

        public enum PartOfDate
        {
            Seconds,
            Minutes,
            Hours,
            DaysOfMonth,
            Months,
            Years,
            Unknown
        }

        public RepeatEveryNode(Token repeatEvery, Token timeToken)
        {
            this.timeToken = timeToken;
        }

        public override RdlSyntaxNode[] Descendants => null;

        public override TextSpan FullSpan => new TextSpan(repeatEvery.Span.Start, timeToken.Span.End - repeatEvery.Span.Start);

        public override bool IsLeaf => true;

        public override Token Token => timeToken;

        public override void Accept(INodeVisitor visitor) => visitor.Visit(this);

        public PartOfDate DatePart
        {
            get
            {
                switch(Token.Value)
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

        public override string ToString() => string.Format("{0} {1}", "repeat every", timeToken.Value);
    }

    public class NumericConsequentRepeatEveryNode : RepeatEveryNode
    {
        private NumericToken number;

        public NumericConsequentRepeatEveryNode(Token repeatEvery, NumericToken number, WordToken partOfDatetime)
            : base(repeatEvery, partOfDatetime)
        {
            this.number = number;
        }

        public override string ToString() => string.Format("{0} {1} {2}", "repeat every", number.Value, timeToken.Value);

        public override int Value => int.Parse(number.Value);
    }
}
