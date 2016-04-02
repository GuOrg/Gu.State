namespace Gu.State
{
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Reflection;

    public abstract class MemberDiff<T> : Diff
        where T : MemberInfo
    {

        private readonly ValueDiff valueDiff;

        public MemberDiff(T memberInfo, object xValue, object yValue)
            : this(memberInfo, new ValueDiff(xValue, yValue))
        {
        }

        public MemberDiff(T memberInfo, ValueDiff diff)
            : base(diff.Diffs)
        {
            this.MemberyInfo = memberInfo;
            this.valueDiff = diff;
        }

        protected T MemberyInfo { get; }

        public object X => this.valueDiff.X;

        public object Y => this.valueDiff.Y;

        public override string ToString()
        {
            return $"{this.MemberyInfo.Name} {this.valueDiff} diffs: {this.Diffs.Count}";
        }

        public override string ToString(string tabString, string newLine)
        {
            if (this.Diffs.Count == 0)
            {
                return $"{this.MemberyInfo.Name} x: {this.X ?? "null"} y: {this.Y ?? "null"}";
            }

            using (var writer = new IndentedTextWriter(new StringWriter(), tabString) { NewLine = newLine })
            {
                writer.Write(this.MemberyInfo.Name);
                this.WriteDiffs(writer);
                return writer.InnerWriter.ToString();
            }
        }

        internal override IndentedTextWriter WriteDiffs(IndentedTextWriter writer)
        {
            if (this.Diffs.Count == 0)
            {
                writer.Write($"{this.MemberyInfo.Name} x: {this.X ?? "null"} y: {this.Y ?? "null"}");
                return writer;
            }

            writer.Write(this.MemberyInfo.Name);
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