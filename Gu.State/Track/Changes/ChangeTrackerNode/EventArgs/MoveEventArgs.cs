namespace Gu.State
{
    internal struct MoveEventArgs
    {
        internal readonly int FromIndex;

        internal readonly int ToIndex;

        public MoveEventArgs(int fromIndex, int toIndex)
        {
            this.FromIndex = fromIndex;
            this.ToIndex = toIndex;
        }
    }
}