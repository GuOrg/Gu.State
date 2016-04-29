namespace Gu.State
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>A diff for an item in a collection.</summary>
    public class IndexDiff : SubDiff
    {
        /// <summary> Initializes a new instance of the <see cref="IndexDiff"/> class.</summary>
        /// <param name="index">The index or key.</param>
        /// <param name="xValue">The x value of the <paramref name="index"/></param>
        /// <param name="yValue">The y value of the <paramref name="index"/></param>
        public IndexDiff(object index, object xValue, object yValue)
            : this(index, new ValueDiff(xValue, yValue))
        {
        }

        /// <summary> Initializes a new instance of the <see cref="IndexDiff"/> class.</summary>
        /// <param name="index">The property.</param>
        /// <param name="valueDiff">The <see cref="ValueDiff"/> for the <paramref name="index"/></param>
        public IndexDiff(object index, ValueDiff valueDiff)
             : base(valueDiff)
        {
            this.Index = index;
        }

        /// <summary>Gets the index or key.</summary>
        public object Index { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{this.Index}] {this.ValueDiff} diffs: {this.Diffs.Count}";
        }

        /// <inheritdoc />
        public override string ToString(string tabString, string newLine)
        {
            if (this.Diffs.Count == 0)
            {
                return $"{this.Index} x: {this.X ?? "null"} y: {this.Y ?? "null"}";
            }

            using (var writer = new IndentedTextWriter(new StringWriter(), tabString) { NewLine = newLine })
            {
                writer.WriteLine(this.Index);
                using (var disposer = BorrowValueDiffReferenceSet())
                {
                    this.WriteDiffs(writer, disposer.Value);
                }

                return writer.InnerWriter.ToString();
            }
        }

        internal override IndentedTextWriter WriteDiffs(IndentedTextWriter writer, HashSet<ValueDiff> written)
        {
            if (!written.Add(this.ValueDiff))
            {
                writer.Write("...");
                return writer;
            }

            if (this.Diffs.Count == 0)
            {
                writer.Write($"[{this.Index}] x: {this.X ?? "null"} y: {this.Y ?? "null"}");
                return writer;
            }

            writer.Write($"[{this.Index}]");
            writer.Indent++;
            foreach (var diff in this.Diffs)
            {
                writer.WriteLine();
                diff.WriteDiffs(writer, written);
            }

            writer.Indent--;
            return writer;
        }
    }
}