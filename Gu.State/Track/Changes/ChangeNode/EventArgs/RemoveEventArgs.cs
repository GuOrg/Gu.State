namespace Gu.State
{
    public struct RemoveEventArgs : IRootChangeEventArgs
    {
        public RemoveEventArgs(int index)
        {
            this.Index = index;
        }

        public int Index { get; }
    }
}