namespace Gu.State
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;

    /// <summary>Multidimensional array rank difference.</summary>
    public class RankDiff : SubDiff
    {
        /// <summary> Initializes a new instance of the <see cref="RankDiff"/> class.</summary>
        /// <param name="x">The left <see cref="Array"/>.</param>
        /// <param name="y">The right <see cref="Array"/>.</param>
        public RankDiff(Array x, Array y)
            : base(new ValueDiff(x, y))
        {
            if (x is null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y is null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            this.XLengths = GetLengths(x);
            this.YLengths = GetLengths(y);
        }

        /// <summary>Gets the lengths in the dimensions for X.</summary>
        public IReadOnlyList<int> XLengths { get; }

        /// <summary>Gets the lengths in the dimensions for Y.</summary>
        public IReadOnlyList<int> YLengths { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{string.Join(",", this.XLengths)}] [{string.Join(",", this.YLengths)}]";
        }

        /// <inheritdoc />
        public override string ToString(string tabString, string newLine)
        {
            return $"x: [{string.Join(",", this.XLengths)}] y: [{string.Join(",", this.YLengths)}]";
        }

        internal override IndentedTextWriter WriteDiffs(IndentedTextWriter writer, HashSet<ValueDiff> written)
        {
            writer.Write($"x: [{string.Join(",", this.XLengths)}] y: [{string.Join(",", this.YLengths)}]");
            return writer;
        }

        private static int[] GetLengths(Array array)
        {
            var lengths = new int[array.Rank];
            for (var i = 0; i < array.Rank; i++)
            {
                lengths[i] = array.GetLength(i);
            }

            return lengths;
        }
    }
}
