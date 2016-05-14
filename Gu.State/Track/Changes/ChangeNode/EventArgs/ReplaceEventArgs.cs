namespace Gu.State
{
    internal struct ReplaceEventArgs : IRootChangeEventArgs
    {
        internal readonly int Index;

        public ReplaceEventArgs(int index)
        {
            this.Index = index;
        }
    }
}