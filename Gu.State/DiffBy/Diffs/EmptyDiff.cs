namespace Gu.State
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;

    /// <summary>This is returned when x and y are equal.</summary>
    public class EmptyDiff : ValueDiff
    {
        /// <summary>Initializes a new instance of the <see cref="EmptyDiff"/> class.</summary>
        /// <param name="xValue">The x value.</param>
        /// <param name="yValue">The y value.</param>
        public EmptyDiff(object xValue, object yValue)
            : base(xValue, yValue)
        {
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Empty";
        }

        /// <inheritdoc />
        public override string ToString(string tabString, string newLine)
        {
            return this.ToString();
        }

        internal override IndentedTextWriter WriteDiffs(IndentedTextWriter writer, HashSet<SubDiff> written)
        {
            return writer;
        }
    }
}