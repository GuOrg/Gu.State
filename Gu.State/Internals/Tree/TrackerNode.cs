namespace Gu.State
{
    internal static partial class TrackerNode
    {
        internal static INode<TTracker> CreateRoot<TTracker>(object source, TTracker tracker)
            where TTracker : ITracker
        {
            return RootNode<TTracker>.CreateRoot(source, tracker);
        }
    }
}