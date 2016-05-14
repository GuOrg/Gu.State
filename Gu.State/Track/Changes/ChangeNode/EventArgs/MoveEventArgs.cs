namespace Gu.State
{
    public struct MoveEventArgs : IRootChangeEventArgs
    {
        public MoveEventArgs(int fromIndex, int toIndex)
        {
            this.FromIndex = fromIndex;
            this.ToIndex = toIndex;
        }

        public int FromIndex { get; }

        public int ToIndex { get; }
    }
}