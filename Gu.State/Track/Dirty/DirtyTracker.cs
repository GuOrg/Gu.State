namespace Gu.State
{
    using System.ComponentModel;

    public abstract class DirtyTracker
    {
        protected static readonly PropertyChangedEventArgs DiffPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Diff));
        protected static readonly PropertyChangedEventArgs IsDirtyPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IsDirty));

        internal DirtyTracker()
        {
        }

        public abstract bool IsDirty { get; }

        public abstract ValueDiff Diff { get; }
    }
}