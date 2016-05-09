namespace Gu.State
{
    using System;

    [Flags]
    internal enum Status
    {
        ThisIsFine = 0,
        NeedsRefresh = 1 << 0,
        IsRefreshing = 1 << 1,
        IsPurging = 1 << 2,
        IsUpdatingDiffs = 1 << 3
    }
}