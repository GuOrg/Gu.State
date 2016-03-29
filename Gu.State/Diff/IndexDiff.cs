namespace Gu.State
{
    public class IndexDiff : Diff
    {
        private readonly ValueDiff valueDiff;

        public IndexDiff(int index, object xValue, object yValue)
        {
            this.Index = index;
            this.valueDiff = new ValueDiff(xValue, yValue);
        }

        public int Index { get; }

        public object X => this.valueDiff.X;

        public object Y => this.valueDiff.Y;

        public override string ToString()
        {
            return $"{this.Index} {this.valueDiff}";
        }
    }
}