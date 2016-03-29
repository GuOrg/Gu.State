namespace Gu.State
{
    using System.Collections.Generic;

    public class ValueDiff : Diff
    {
        public ValueDiff(object xValue, object yValue)
        {
            this.X = xValue;
            this.Y = yValue;
        }

        public ValueDiff(object xValue, object yValue, IReadOnlyCollection<Diff> diffs)
            : base(diffs)
        {
            this.X = xValue;
            this.Y = yValue;
        }

        public object X { get; }

        public object Y { get; }

        public override string ToString()
        {
            return $"x: {this.X ?? "null"} y: {this.Y ?? "null"}";
        }
    }
}