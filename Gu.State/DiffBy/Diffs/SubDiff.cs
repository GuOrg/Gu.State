namespace Gu.State
{
    /// <summary>A node in a diff tree.</summary>
    public abstract class SubDiff : Diff
    {
        internal readonly ValueDiff ValueDiff;

        /// <summary>Initializes a new instance of the <see cref="SubDiff"/> class.</summary>
        /// <param name="valueDiff">The diff.</param>
        protected SubDiff(ValueDiff valueDiff)
            : base(valueDiff.Diffs)
        {
            this.ValueDiff = valueDiff;
        }

        /// <inheritdoc />
        public override bool IsEmpty => this.ValueDiff.IsEmpty;

        /// <summary>Gets the x value.</summary>
        public object X => this.ValueDiff.X;

        /// <summary>Gets the y value.</summary>
        public object Y => this.ValueDiff.Y;
    }
}