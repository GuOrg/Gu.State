namespace Gu.State
{
    internal class SubDiffBuilder : DiffBuilder
    {
        private readonly DiffBuilderRoot root;

        public SubDiffBuilder(DiffBuilderRoot root, object x, object y)
            : base(x, y)
        {
            this.root = root;
        }

        internal override bool TryAdd(object x, object y, out DiffBuilder subDiffBuilder)
        {
            if (this.root.TryGetSubBuilder(x, y, out subDiffBuilder))
            {
                return false;
            }

            subDiffBuilder = new SubDiffBuilder(this.root, x, y);
            return true;
        }
    }
}