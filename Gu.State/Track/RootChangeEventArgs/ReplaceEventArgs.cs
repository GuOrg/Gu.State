namespace Gu.State
{
    /// <summary>This is raised when an element was replaced in a notifying collection.</summary>
    public struct ReplaceEventArgs : IRootChangeEventArgs
    {
        internal ReplaceEventArgs(int index)
        {
            this.Index = index;
        }

        /// <summary>Gets the index at which an element was replaced. </summary>
        public int Index { get; }
    }
}