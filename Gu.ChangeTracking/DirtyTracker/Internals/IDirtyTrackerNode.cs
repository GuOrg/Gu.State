namespace Gu.ChangeTracking
{
    using System;
    using System.Reflection;

    internal interface IDirtyTrackerNode : IDisposable
    {
        DirtyTrackerSettings Settings { get; }

        bool IsDirty { get; }

        PropertyInfo PropertyInfo { get; }

        void Update(IDirtyTrackerNode child);
    }
}