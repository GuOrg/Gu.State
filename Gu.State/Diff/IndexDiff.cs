namespace Gu.State
{
    public class IndexDiff : Diff
    {
        public IndexDiff(int index, object xValue, object yValue)
            : base(EmptyDiffs)
        {
            this.Index = index;
            this.X = xValue;
            this.Y = yValue;
        }

        public int Index { get; }

        public object X { get; }

        public object Y { get; }
    }
}