namespace Gu.State
{
    internal struct RemoveEventArgs
    {
        internal readonly int Index;

        public RemoveEventArgs(int index)
        {
            this.Index = index;
        }
    }
}