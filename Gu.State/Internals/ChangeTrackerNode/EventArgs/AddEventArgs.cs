namespace Gu.State
{
    internal struct AddEventArgs
    {
        public AddEventArgs(int index, object item)
        {
            this.Index = index;
            this.Item = item;
        }

        public int Index { get; }

        public object Item { get; }
    }
}