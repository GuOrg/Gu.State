namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal abstract class DiffBuilder
    {
        private readonly List<SubDiff> diffs = new List<SubDiff>();
        private readonly List<Func<SubDiff>> builders = new List<Func<SubDiff>>();

        internal bool IsEmpty => this.Diffs == null || !this.Diffs.Any();

        internal IEnumerable<SubDiff> Diffs => this.diffs.Concat(this.BuildDiffs());

        internal abstract bool TryAdd(object x, object y, out SubBuilder subBuilder);

        internal void Add(SubDiff subDiff)
        {
            this.diffs.Add(subDiff);
        }

        internal void Add(Func<SubDiff> builder)
        {
            this.builders.Add(builder);
        }

        private IEnumerable<SubDiff> BuildDiffs()
        {
            return this.builders
                       .Select(b => b())
                       .Where(x => x != null);
        }
    }
}
