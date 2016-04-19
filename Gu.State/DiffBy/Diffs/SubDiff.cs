namespace Gu.State
{
    /// <summary>A node in a diff tree.</summary>
    public abstract class SubDiff : Diff
    {
        internal readonly ValueDiff ValueDiff;

        protected SubDiff(ValueDiff valueDiff)
            : base(valueDiff.Diffs)
        {
            this.ValueDiff = valueDiff;
        }

        public object X => this.ValueDiff.X;

        public object Y => this.ValueDiff.Y;
    }
}