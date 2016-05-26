namespace Gu.State
{
    /// <summary>This is raised when an element is removed from a notifying collection.</summary>
    public struct RemoveEventArgs : IRootChangeEventArgs
    {
        internal RemoveEventArgs(int index)
        {
            this.Index = index;
        }

        /// <summary>Gets the index at which an element was removed. </summary>
        public int Index { get; }
    }
}