namespace Gu.State
{
    internal struct AddEventArgs : IRootChangeEventArgs
    {
        internal readonly int Index;

        public AddEventArgs(int index)
        {
            this.Index = index;
        }
    }
}