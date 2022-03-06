namespace Gu.State
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>A value difference X != Y. </summary>
    public class ValueDiff : Diff
    {
        /// <summary>Initializes a new instance of the <see cref="ValueDiff"/> class.</summary>
        /// <param name="xValue">The x value.</param>
        /// <param name="yValue">The y value.</param>
        /// <param name="diffs">The nested diffs.</param>
        public ValueDiff(object xValue, object yValue, IReadOnlyCollection<SubDiff> diffs = null) : base(diffs)
        {
            this.X = xValue;
            this.Y = yValue;
        }

        /// <summary>
        /// Gets a value indicating whether the diff is empty.
        /// always false.
        /// </summary>
        public override bool IsEmpty => false;

        /// <summary>Gets the X value.</summary>
        public object X { get; }

        /// <summary>Gets the Y value.</summary>
        public object Y { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"x: {this.X.ToInvariantOrNullString()} y: {this.Y.ToInvariantOrNullString()} diffs: {this.Diffs.Count}";
        }

        /// <inheritdoc />
        public override string ToString(string tabString, string newLine)
        {
            if (this.Diffs.Count == 0)
            {
                return $"{this.X.GetType().PrettyName()} x: {this.X.ToInvariantOrNullString()} y: {this.Y.ToInvariantOrNullString()}";
            }

            using var stringWriter = new StringWriter();
            using var writer = new IndentedTextWriter(stringWriter, tabString) { NewLine = newLine };
            writer.Write(this.X.GetType().PrettyName());
            using (var disposer = BorrowValueDiffReferenceSet())
            {
                disposer.Value.Add(this);
                this.WriteDiffs(writer, disposer.Value);
            }

            return writer.InnerWriter.ToString();
        }

        internal override IndentedTextWriter WriteDiffs(IndentedTextWriter writer, HashSet<ValueDiff> written)
        {
            writer.Indent++;
            foreach (var diff in this.Diffs)
            {
                writer.WriteLine();
                _ = diff.WriteDiffs(writer, written);
            }

            writer.Indent--;
            return writer;
        }
    }
}
