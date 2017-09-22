namespace Gu.State
{
    using System.CodeDom.Compiler;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>A node in a diff tree.</summary>
    public abstract class Diff
    {
        private static readonly IReadOnlyCollection<SubDiff> Empty = new SubDiff[0];

        internal Diff(IReadOnlyCollection<SubDiff> diffs = null)
        {
            this.Diffs = diffs ?? Empty;
        }

        /// <summary>Gets a value indicating whether the diff is empty.</summary>
        public abstract bool IsEmpty { get; }

        /// <summary>Gets the diffs for properties and indexes.</summary>
        public IReadOnlyCollection<SubDiff> Diffs { get; }

        /// <summary>
        /// Creates a report for all diffs
        /// </summary>
        /// <param name="tabString">The string to use for indentation.</param>
        /// <param name="newLine">The newline ex: <see cref="System.Environment.NewLine"/></param>
        /// <returns>A report with all diffs.</returns>
        public abstract string ToString(string tabString, string newLine);

        internal static Disposer<HashSet<ValueDiff>> BorrowValueDiffReferenceSet()
        {
            return ValueDiffSetPool.Borrow();
        }

        internal abstract IndentedTextWriter WriteDiffs(IndentedTextWriter writer, HashSet<ValueDiff> written);

        private static class ValueDiffSetPool
        {
            private static readonly ConcurrentQueue<HashSet<ValueDiff>> Pool = new ConcurrentQueue<HashSet<ValueDiff>>();

            internal static Disposer<HashSet<ValueDiff>> Borrow()
            {
                if (Pool.TryDequeue(out var set))
                {
                    return new Disposer<HashSet<ValueDiff>>(set, Return);
                }

                return new Disposer<HashSet<ValueDiff>>(new HashSet<ValueDiff>(), Return);
            }

            private static void Return(HashSet<ValueDiff> set)
            {
                set.Clear();
                Pool.Enqueue(set);
            }
        }
    }
}
