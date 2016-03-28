namespace Gu.State
{
    using System;

    public class TypeDiff : Diff
    {
        private readonly ValueDiff valueDiff;

        public TypeDiff(Type type, object xValue, object yValue)
            : base(EmptyDiffs)
        {
            this.Type = type;
            this.valueDiff = new ValueDiff(xValue, yValue);
        }

        public TypeDiff(Type type, object xValue, object yValue, Diff diff)
            : base(diff.Diffs)
        {
            this.Type = type;
            this.valueDiff = new ValueDiff(xValue, yValue);
        }

        public Type Type { get; }

        public override bool IsEmpty => false;

        public object X => this.valueDiff.X;

        public object Y => this.valueDiff.Y;

        public override string ToString()
        {
            if (this.Diffs.Count == 0)
            {
                return $"{this.Type.Name} {this.valueDiff}";
            }

            throw new NotImplementedException("message");
        }
    }
}