using RDL.Parser.Tokens;

namespace RDL.Parser.Nodes
{
    public class NumericConsequentRepeatEveryNode : RepeatEveryNode
    {
        private readonly NumericToken _number;

        public NumericConsequentRepeatEveryNode(Token repeatEvery, NumericToken number, WordToken partOfDatetime)
            : base(repeatEvery, partOfDatetime)
        {
            _number = number;
        }

        public override int Value => int.Parse(_number.Value);

        public override string ToString() => $"repeat every {_number.Value} {base.Value}";
    }
}