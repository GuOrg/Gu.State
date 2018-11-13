namespace Gu.State
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    /// <summary>A diff for a member.</summary>
    /// <typeparam name="TMember">The member.</typeparam>
    public abstract class MemberDiff<TMember> : MemberDiff
        where TMember : MemberInfo
    {
        /// <summary> Initializes a new instance of the <see cref="MemberDiff{T}"/> class.</summary>
        /// <param name="memberInfo">The member.</param>
        /// <param name="xValue">The x value of the <paramref name="memberInfo"/>.</param>
        /// <param name="yValue">The y value of the <paramref name="memberInfo"/>.</param>
        protected MemberDiff(TMember memberInfo, object xValue, object yValue)
            : this(memberInfo, new ValueDiff(xValue, yValue))
        {
        }

        /// <summary> Initializes a new instance of the <see cref="MemberDiff{T}"/> class.</summary>
        /// <param name="memberInfo">The member.</param>
        /// <param name="diff">The <see cref="ValueDiff"/> for the <paramref name="memberInfo"/>.</param>
        protected MemberDiff(TMember memberInfo, ValueDiff diff)
            : base(memberInfo, diff)
        {
        }

        /// <summary>Gets the member.</summary>
        protected new TMember MemberInfo => (TMember)base.MemberInfo;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.MemberInfo.Name} {this.ValueDiff} diffs: {this.Diffs.Count}";
        }

        /// <inheritdoc />
        public override string ToString(string tabString, string newLine)
        {
            if (this.Diffs.Count == 0)
            {
                return $"{this.MemberInfo.Name} x: {this.X.ToInvariantOrNullString()} y: {this.Y.ToInvariantOrNullString()}";
            }

            using (var writer = new IndentedTextWriter(new StringWriter(), tabString) { NewLine = newLine })
            {
                using (var disposer = BorrowValueDiffReferenceSet())
                {
                    writer.Write(this.MemberInfo.Name);
                    this.WriteDiffs(writer, disposer.Value);
                }

                return writer.InnerWriter.ToString();
            }
        }

        internal override IndentedTextWriter WriteDiffs(IndentedTextWriter writer, HashSet<ValueDiff> written)
        {
            if (!written.Add(this.ValueDiff))
            {
                writer.Write($"{this.MemberInfo.Name} ...");
                return writer;
            }

            if (this.Diffs.Count == 0)
            {
                writer.Write($"{this.MemberInfo.Name} x: {this.X.ToInvariantOrNullString()} y: {this.Y.ToInvariantOrNullString()}");
                return writer;
            }

            writer.Write(this.MemberInfo.Name);
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