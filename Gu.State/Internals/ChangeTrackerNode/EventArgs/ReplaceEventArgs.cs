namespace Gu.State
{
    internal struct ReplaceEventArgs
    {
        internal readonly int Index;

        public ReplaceEventArgs(int index)
        {
            this.Index = index;
        }
    }
}