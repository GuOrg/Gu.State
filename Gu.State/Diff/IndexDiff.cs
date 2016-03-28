namespace Gu.State
{
    using System;

    public class IndexDiff : Diff
    {
        private readonly ValueDiff valueDiff;

        public IndexDiff(int index, object xValue, object yValue)
            : base(EmptyDiffs)
        {
            this.Index = index;
            this.valueDiff = new ValueDiff(xValue, yValue);
        }

        public int Index { get; }

        public object X => this.valueDiff.X;

        public object Y => this.valueDiff.Y;

        public override string ToString()
        {
            if (this.Diffs.Count == 0)
            {
                return $"{this.Index} {this.valueDiff}";
            }

            throw new NotImplementedException("message");
        }
    }
}