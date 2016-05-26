namespace Gu.State
{
    public struct AddEventArgs : IRootChangeEventArgs
    {
        public AddEventArgs(int index)
        {
            this.Index = index;
        }

        public int Index { get; }
    }
}