namespace Gu.State
{
    using System.CodeDom.Compiler;
    using System.IO;

    public class IndexDiff : SubDiff
    {
        public IndexDiff(object index, object xValue, object yValue)
            : this(index, new ValueDiff(xValue, yValue))
        {
        }

        public IndexDiff(object index, ValueDiff valueDiff)
             : base(valueDiff)
        {
            this.Index = index;
        }

        public object Index { get; }

        public override string ToString()
        {
            return $"[{this.Index}] {this.ValueDiff} diffs: {this.Diffs.Count}";
        }

        public override string ToString(string tabString, string newLine)
        {
            if (this.Diffs.Count == 0)
            {
                return $"{this.Index} x: {this.X ?? "null"} y: {this.Y ?? "null"}";
            }

            using (var writer = new IndentedTextWriter(new StringWriter(), tabString) { NewLine = newLine })
            {
                writer.WriteLine(this.Index);
                this.WriteDiffs(writer);
                return writer.InnerWriter.ToString();
            }
        }

        internal override IndentedTextWriter WriteDiffs(IndentedTextWriter writer)
        {
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
                diff.WriteDiffs(writer);
            }

            writer.Indent--;
            return writer;
        }
    }
}