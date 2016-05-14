namespace Gu.State
{
    internal static class TrackerChangedEventArgs
    {
        internal static TrackerChangedEventArgs<T> Create<T>(T root, object memberOrIndex)
        {
            return new TrackerChangedEventArgs<T>(root, memberOrIndex);
        }
    }
}