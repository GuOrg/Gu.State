namespace Gu.State
{
    public static class RootChangeEventArgs
    {
        internal static RootChangeEventArgs<T> Create<T, TEventArgs>(T sender, TEventArgs eventArgs)
           where TEventArgs : IRootChangeEventArgs
        {
            return new RootChangeEventArgs<T>(sender, eventArgs);
        }
    }
}