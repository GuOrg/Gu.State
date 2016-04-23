namespace Gu.State
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    /// <summary>A diff for a member.</summary>
    /// <typeparam name="T">The member.</typeparam>
    public abstract class MemberDiff<T> : SubDiff
        where T : MemberInfo
    {
        /// <summary> Initializes a new instance of the <see cref="MemberDiff{T}"/> class.</summary>
        /// <param name="memberInfo">The member.</param>
        /// <param name="xValue">The x value of the <paramref name="memberInfo"/></param>
        /// <param name="yValue">The y value of the <paramref name="memberInfo"/></param>
        protected MemberDiff(T memberInfo, object xValue, object yValue)
            : this(memberInfo, new ValueDiff(xValue, yValue))
        {
        }

        /// <summary> Initializes a new instance of the <see cref="MemberDiff{T}"/> class.</summary>
        /// <param name="memberInfo">The member.</param>
        /// <param name="diff">The <see cref="ValueDiff"/> for the <paramref name="memberInfo"/></param>
        protected MemberDiff(T memberInfo, ValueDiff diff)
            : base(diff)
        {
            this.MemberyInfo = memberInfo;
        }

        /// <summary>Gets the member.</summary>
        protected T MemberyInfo { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.MemberyInfo.Name} {this.ValueDiff} diffs: {this.Diffs.Count}";
        }

        /// <inheritdoc />
        public override string ToString(string tabString, string newLine)
        {
            if (this.Diffs.Count == 0)
            {
                return $"{this.MemberyInfo.Name} x: {this.X ?? "null"} y: {this.Y ?? "null"}";
            }

            using (var writer = new IndentedTextWriter(new StringWriter(), tabString) { NewLine = newLine })
            {
                using (var disposer = BorrowReferenceList())
                {
                    writer.Write(this.MemberyInfo.Name);
                    this.WriteDiffs(writer, disposer.Value);
                }

                return writer.InnerWriter.ToString();
            }
        }

        internal override IndentedTextWriter WriteDiffs(IndentedTextWriter writer, HashSet<ValueDiff> written)
        {
            if (!written.Add(this.ValueDiff))
            {
                writer.Write($"{this.MemberyInfo.Name} ...");
                return writer;
            }

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
                diff.WriteDiffs(writer, written);
            }

            writer.Indent--;
            return writer;
        }
    }
}