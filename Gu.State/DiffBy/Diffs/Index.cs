namespace Gu.State
{
    internal struct Index
    {
        internal Index(int[] indices)
        {
            this.Indices = indices;
        }

        internal int[] Indices { get; }

        public override string ToString()
        {
            return string.Join(",", this.Indices);
        }
    }
}
