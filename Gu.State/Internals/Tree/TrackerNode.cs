namespace Gu.State
{
    internal static partial class TrackerNode
    {
        internal static INode<TKey, TTracker> CreateRoot<TKey, TTracker>(TKey key, TTracker tracker)
            where TKey : IReference
            where TTracker : ITracker
        {
            return RootNode<TKey, TTracker>.CreateRoot(key, tracker);
        }
    }
}