namespace Gu.State
{
    public struct ReplaceEventArgs : IRootChangeEventArgs
    {
        public ReplaceEventArgs(int index)
        {
            this.Index = index;
        }

        public int Index { get; }
    }
}