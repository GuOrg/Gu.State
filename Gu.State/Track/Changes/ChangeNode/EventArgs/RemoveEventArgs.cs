namespace Gu.State
{
    internal struct RemoveEventArgs : IRootChangeEventArgs
    {
        internal readonly int Index;

        public RemoveEventArgs(int index)
        {
            this.Index = index;
        }
    }
}