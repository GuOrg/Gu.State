namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal sealed class DiffBuilder : IDisposable
    {
        private readonly List<SubDiff> diffs = new List<SubDiff>();
        private readonly List<Disposer<DiffBuilder>> disposers = new List<Disposer<DiffBuilder>>();
        private readonly ValueDiff valueDiff;
        private bool disposed;

        private DiffBuilder(object x, object y)
        {
            this.valueDiff = new ValueDiff(x, y, this.diffs);
        }

        private bool IsEmpty => this.diffs.All(d => d.IsEmpty);

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            foreach (var disposer in this.disposers)
            {
                disposer.Dispose();
            }

            this.disposers.Clear();
        }

        internal static IRefCounted<DiffBuilder> Create(object x, object y, IMemberSettings settings)
        {
            return TrackerCache.GetOrAdd(x, y, settings, pair => new DiffBuilder(pair.X, pair.Y));
        }

        internal static bool TryCreate(object x, object y, IMemberSettings settings, out IRefCounted<DiffBuilder> subDiffBuilder)
        {
            bool created;
            subDiffBuilder = TrackerCache.GetOrAdd(
                ReferencePair.GetOrCreate(x, y),
                settings,
                pair => new DiffBuilder(pair.X, pair.Y),
                out created);

            return created;
        }

        internal void Add(SubDiff subDiff)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            this.diffs.Add(subDiff);
        }

        internal void AddLazy(MemberInfo member, DiffBuilder builder)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            this.diffs.Add(MemberDiff.Create(member, builder.valueDiff));
        }

        internal void AddLazy(object index, DiffBuilder builder)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            this.diffs.Add(new IndexDiff(index, builder.valueDiff));
        }

        internal ValueDiff CreateValueDiff()
        {
            Debug.Assert(!this.disposed, "this.disposed");
            this.PurgeEmpty();
            return this.IsEmpty ? null : this.valueDiff;
        }

        private void PurgeEmpty()
        {
            throw new NotImplementedException("message");
            
            //foreach (var builder in this.builderCache.Values)
            //{
            //    if (builder.IsEmpty)
            //    {
            //        builder.Empty?.Invoke(builder, EventArgs.Empty);
            //    }
            //}
        }
    }
}
