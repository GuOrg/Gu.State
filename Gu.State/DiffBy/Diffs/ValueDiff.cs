namespace Gu.State
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>A value difference X != Y </summary>
    public class ValueDiff : Diff
    {
        private bool? isEmpty;

        /// <summary>Initializes a new instance of the <see cref="ValueDiff"/> class.</summary>
        /// <param name="xValue">The x value.</param>
        /// <param name="yValue">The y value.</param>
        public ValueDiff(object xValue, object yValue)
        {
            this.X = xValue;
            this.Y = yValue;
        }

        /// <summary>Initializes a new instance of the <see cref="ValueDiff"/> class.</summary>
        /// <param name="xValue">The x value.</param>
        /// <param name="yValue">The y value.</param>
        /// <param name="diffs">The nested diffs.</param>
        public ValueDiff(object xValue, object yValue, IReadOnlyList<SubDiff> diffs)
            : base(diffs)
        {
            this.X = xValue;
            this.Y = yValue;
        }

        public override bool IsEmpty => (bool)(this.isEmpty ?? (this.isEmpty = !this.HasNodeDiff()));

        /// <summary>Gets the X value.</summary>
        public object X { get; }

        /// <summary>Gets the Y value.</summary>
        public object Y { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"x: {this.X ?? "null"} y: {this.Y ?? "null"} diffs: {this.Diffs.Count}";
        }

        /// <inheritdoc />
        public override string ToString(string tabString, string newLine)
        {
            if (this.Diffs.Count == 0)
            {
                return $"{this.X.GetType().PrettyName()} x: {this.X ?? "null"} y: {this.Y ?? "null"}";
            }

            using (var writer = new IndentedTextWriter(new StringWriter(), tabString) { NewLine = newLine })
            {
                writer.Write(this.X.GetType().PrettyName());
                using (var disposer = BorrowValueDiffReferenceSet())
                {
                    disposer.Value.Add(this);
                    this.WriteDiffs(writer, disposer.Value);
                }

                return writer.InnerWriter.ToString();
            }
        }

        internal override IndentedTextWriter WriteDiffs(IndentedTextWriter writer, HashSet<ValueDiff> written)
        {
            writer.Indent++;
            foreach (var diff in this.Diffs)
            {
                writer.WriteLine();
                diff.WriteDiffs(writer, written);
            }

            writer.Indent--;
            return writer;
        }

        private bool HasNodeDiff()
        {
            if (this.Diffs.Count == 0)
            {
                return true;
            }

            using (var diffs = BorrowValueDiffReferenceSet())
            {
                return this.HasNodeDiff(diffs.Value);
            }
        }

        private bool HasNodeDiff(HashSet<ValueDiff> @checked)
        {
            foreach (var subDiff in this.Diffs)
            {
                if (!@checked.Add(subDiff.ValueDiff))
                {
                    continue;
                }

                if (subDiff.Diffs.Count == 0)
                {
                    return true;
                }

                if (subDiff.ValueDiff.HasNodeDiff(@checked))
                {
                    return true;
                }
            }

            return false;
        }
    }
}