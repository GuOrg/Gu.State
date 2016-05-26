namespace Gu.State
{
    internal static class RootChangeEventArgs
    {
        internal static RootChangeEventArgs<T> Create<T, TEventArgs>(T sender, TEventArgs eventArgs)
           where TEventArgs : IRootChangeEventArgs
        {
            return new RootChangeEventArgs<T>(sender, eventArgs);
        }
    }
}