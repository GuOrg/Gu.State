namespace Gu.State
{
    public class ValueDiff : Diff
    {
        public ValueDiff(object xValue, object yValue)
            : base(EmptyDiffs)
        {
            this.X = xValue;
            this.Y = yValue;
        }

        public override bool IsEmpty => false;

        public object X { get; }

        public object Y { get; }
    }
}