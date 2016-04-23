namespace Gu.State
{
    internal class SubDiffBuilder : DiffBuilder
    {
        private readonly DiffBuilderRoot root;

        public SubDiffBuilder(DiffBuilderRoot root, object x, object y)
            : base(x, y, root)
        {
            this.root = root;
        }

        internal override bool TryAdd(object x, object y, out DiffBuilder subDiffBuilder)
        {
            if (this.root.TryGetBuilder(x, y, out subDiffBuilder))
            {
                subDiffBuilder.Empty += this.OnSubBuilderEmpty;
                return false;
            }

            subDiffBuilder = new SubDiffBuilder(this.root, x, y);
            subDiffBuilder.Empty += this.OnSubBuilderEmpty;
            return true;
        }
    }
}