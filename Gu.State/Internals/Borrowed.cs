namespace Gu.State
{
    using System;

    internal static class Borrowed
    {
        internal static IBorrowed<T> Create<T>(T value, Action<T> @return)
        {
            return new Disposer<T>(value, @return);
        }
    }
}