namespace Gu.State
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;

    /// <summary>A node in a diff tree.</summary>
    public abstract class Diff
    {
        private static readonly IReadOnlyList<SubDiff> Empty = new SubDiff[0];

        internal Diff(IReadOnlyList<SubDiff> diffs = null)
        {
            this.Diffs = diffs ?? Empty;
        }

        /// <summary>Gets the diffs for properties and indexes.</summary>
        public IReadOnlyList<SubDiff> Diffs { get; }

        /// <summary>
        /// Creates a report for all diffs
        /// </summary>
        /// <param name="tabString">The string to use for indentation.</param>
        /// <param name="newLine">The newline ex: <see cref="System.Environment.NewLine"/></param>
        /// <returns>A report with all diffs.</returns>
        public abstract string ToString(string tabString, string newLine);

        internal static Disposer<HashSet<SubDiff>> BorrowReferenceList()
        {
            return ReferenceSetPool<SubDiff>.Borrow();
        }

        internal abstract IndentedTextWriter WriteDiffs(IndentedTextWriter writer, HashSet<SubDiff> written);
    }
}
