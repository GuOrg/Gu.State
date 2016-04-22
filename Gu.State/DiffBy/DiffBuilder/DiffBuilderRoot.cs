namespace Gu.State
{
    internal class DiffBuilderRoot : DiffBuilder
    {
        private readonly ReferenceHandling referenceHandling;

        internal DiffBuilderRoot(object x, object y, ReferenceHandling referenceHandling)
            : base(x, y)
        {
            this.referenceHandling = referenceHandling;
        }

        internal override bool TryAdd(object x, object y, out DiffBuilder subDiffBuilder)
        {
            if (this.TryGetSubBuilder(x, y, out subDiffBuilder))
            {
                return false;
            }

            subDiffBuilder = new SubDiffBuilder(this, x, y);
            return true;
        }
    }
}