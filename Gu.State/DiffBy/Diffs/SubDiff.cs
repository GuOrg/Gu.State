namespace Gu.State
{
    /// <summary>A node in a diff tree.</summary>
    public abstract class SubDiff : Diff
    {
        internal readonly ValueDiff ValueDiff;

        /// <summary>Initializes a new instance of the <see cref="SubDiff"/> class.</summary>
        /// <param name="valueDiff">The diff.</param>
#pragma warning disable RS0022 // Constructor make non-inheritable base class inheritable
        protected SubDiff(ValueDiff valueDiff)
#pragma warning restore RS0022 // Constructor make non-inheritable base class inheritable
            : base(valueDiff?.Diffs)
        {
            this.ValueDiff = valueDiff ?? throw new System.ArgumentNullException(nameof(valueDiff));
        }

        /// <inheritdoc />
        public override bool IsEmpty => this.ValueDiff.IsEmpty;

        /// <summary>Gets the x value.</summary>
        public object X => this.ValueDiff.X;

        /// <summary>Gets the y value.</summary>
        public object Y => this.ValueDiff.Y;
    }
}
