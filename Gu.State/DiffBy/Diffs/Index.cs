namespace Gu.State
{
    public struct Index
    {
        public Index(int[] indices)
        {
            this.Indices = indices;
        }

        public int[] Indices { get; }

        public override string ToString()
        {
            return string.Join(",", this.Indices);
        }
    }
}