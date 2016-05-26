namespace Gu.State
{
    public abstract class GraphChangeEventArgs<T> : TrackerChangedEventArgs<T>
    {
        protected GraphChangeEventArgs(T node, TrackerChangedEventArgs<T> previous = null)
             : base(node, previous)
        {
        }
    }
}