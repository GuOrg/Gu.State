namespace Gu.State
{
    using System.CodeDom.Compiler;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public abstract class Diff
    {
        private static readonly IReadOnlyList<SubDiff> Empty = new SubDiff[0];

        internal Diff(IReadOnlyList<SubDiff> diffs = null)
        {
            this.Diffs = diffs ?? Empty;
        }

        public IReadOnlyList<SubDiff> Diffs { get; }

        public abstract string ToString(string tabString, string newLine);

        internal abstract IndentedTextWriter WriteDiffs(IndentedTextWriter writer, List<SubDiff> written);

        internal static Disposer<List<SubDiff>> BorrowReferenceList()
        {
            return ReferenceListPool.Borrow();
        }

        private static class ReferenceListPool
        {
            private static readonly ConcurrentQueue<List<SubDiff>> Pool = new ConcurrentQueue<List<SubDiff>>();

            internal static Disposer<List<SubDiff>> Borrow()
            {
                List<SubDiff> list;
                if (Pool.TryDequeue(out list))
                {
                    return new Disposer<List<SubDiff>>(list, Return);
                }

                return new Disposer<List<SubDiff>>(new List<SubDiff>(), Return);
            }

            private static void Return(List<SubDiff> list)
            {
                list.Clear();
                Pool.Enqueue(list);
            }
        }
    }
}
