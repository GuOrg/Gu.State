namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    internal class DiffBuilderRoot : DiffBuilder
    {
        private readonly ReferenceHandling referenceHandling;

        internal DiffBuilderRoot(object x, object y, ReferenceHandling referenceHandling)
            : base(x, y, new Dictionary<ReferencePair, DiffBuilder>())
        {
            this.referenceHandling = referenceHandling;
        }

        internal override bool TryAdd(object x, object y, out DiffBuilder subDiffBuilder)
        {
            if (this.TryGetBuilder(x, y, out subDiffBuilder))
            {
                subDiffBuilder.Empty += this.OnSubBuilderEmpty;
                return false;
            }

            subDiffBuilder = new SubDiffBuilder(this, x, y);
            subDiffBuilder.Empty += this.OnSubBuilderEmpty;
            return true;
        }
    }
}