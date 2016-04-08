namespace Gu.State
{
    public abstract class SubDiff : Diff
    {
        internal readonly ValueDiff ValueDiff;

        public SubDiff(ValueDiff valueDiff)
            : base(valueDiff.Diffs)
        {
            this.ValueDiff = valueDiff;
        }

        public object X => this.ValueDiff.X;

        public object Y => this.ValueDiff.Y;
    }
}