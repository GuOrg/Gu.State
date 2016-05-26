namespace Gu.State
{
    /// <summary>This is raised when an element was added to a notifying collection.</summary>
    public struct AddEventArgs : IRootChangeEventArgs
    {
        internal AddEventArgs(int index)
        {
            this.Index = index;
        }

        /// <summary>Gets the index at which an element was added. </summary>
        public int Index { get; }
    }
}