namespace Gu.State
{
    internal class SubBuilder : DiffBuilder
    {
        private readonly DiffBuilderRoot root;

        public SubBuilder(DiffBuilderRoot root)
        {
            this.root = root;
        }

        internal override bool TryAdd(object x, object y, out SubBuilder subBuilder)
        {
            if (this.root.TryGetSubBuilder(x, y, out subBuilder))
            {
                return false;
            }

            subBuilder = new SubBuilder(this.root);
            this.root.AddSubBuilderToCache(x, y, subBuilder);
            return true;
        }
    }
}