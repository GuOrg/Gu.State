namespace Gu.State
{
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Reflection;

    public class PropertyDiff : Diff
    {
        private readonly ValueDiff valueDiff;

        public PropertyDiff(PropertyInfo propertyInfo, object xValue, object yValue)
            : this(propertyInfo, new ValueDiff(xValue, yValue))
        {
        }

        public PropertyDiff(PropertyInfo propertyInfo, ValueDiff diff)
            : base(diff.Diffs)
        {
            this.PropertyInfo = propertyInfo;
            this.valueDiff = diff;
        }

        public PropertyInfo PropertyInfo { get; }

        public object X => this.valueDiff.X;

        public object Y => this.valueDiff.Y;

        public override string ToString()
        {
            return $"{this.PropertyInfo.Name} {this.valueDiff} diffs: {this.Diffs.Count}";
        }

        public override string ToString(string tabString, string newLine)
        {
            if (this.Diffs.Count == 0)
            {
                return $"{this.PropertyInfo.Name} x: {this.X ?? "null"} y: {this.Y ?? "null"}";
            }

            using (var writer = new IndentedTextWriter(new StringWriter(), tabString) { NewLine = newLine })
            {
                writer.Write(this.PropertyInfo.Name);
                this.WriteDiffs(writer);
                return writer.InnerWriter.ToString();
            }
        }

        internal override IndentedTextWriter WriteDiffs(IndentedTextWriter writer)
        {
            if (this.Diffs.Count == 0)
            {
                writer.Write($"{this.PropertyInfo.Name} x: {this.X ?? "null"} y: {this.Y ?? "null"}");
                return writer;
            }

            writer.Write(this.PropertyInfo.Name);
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