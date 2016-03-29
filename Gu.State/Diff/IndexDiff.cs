namespace Gu.State
{
    using System.CodeDom.Compiler;
    using System.IO;

    public class IndexDiff : Diff
    {
        private readonly ValueDiff valueDiff;

        public IndexDiff(int index, object xValue, object yValue)
        {
            this.Index = index;
            this.valueDiff = new ValueDiff(xValue, yValue);
        }

        public int Index { get; }

        public object X => this.valueDiff.X;

        public object Y => this.valueDiff.Y;

        public override string ToString()
        {
            return $"[{this.Index}] {this.valueDiff} diffs: {this.Diffs.Count}";
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