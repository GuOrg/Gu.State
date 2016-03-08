namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    /// <summary>
    /// Tracks if there is a difference between the properties of x and y
    /// </summary>
    [DebuggerDisplay("DirtyTracker<{typeof(T).Name}> IsDirty: {IsDirty}")]
    public partial class DirtyTracker<T> : INotifyPropertyChanged, IDirtyTracker
        where T : class, INotifyPropertyChanged
    {
        private readonly HashSet<PropertyInfo> diff = new HashSet<PropertyInfo>();
        private readonly PropertiesDirtyTracker propertiesTrackers;
        private readonly ItemsDirtyTracker itemsTrackers;

        private bool disposed;

        public DirtyTracker(T x, T y, PropertiesSettings settings)
        {
            Ensure.NotNull(x, nameof(x));
            Ensure.NotNull(y, nameof(y));
            Ensure.NotSame(x, y, nameof(x), nameof(y));
            Ensure.SameType(x, y);
            Track.VerifyCanTrackIsDirty<T>(settings);
            this.Settings = settings;
            this.propertiesTrackers = PropertiesDirtyTracker.Create(x, y, this);
            this.itemsTrackers = ItemsDirtyTracker.Create(x, y, this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDirty => this.diff.Count != 0;

        PropertyInfo IDirtyTrackerNode.PropertyInfo => null;

        public IEnumerable<PropertyInfo> Diff => this.diff;

        public PropertiesSettings Settings { get; }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            GC.SuppressFinalize(this);
            this.disposed = true;
            this.Dispose(true);
        }

        void IDirtyTracker.Update(IDirtyTrackerNode child)
        {
            var before = this.diff.Count;
            if (child.IsDirty)
            {
                this.diff.Add(child.PropertyInfo);
            }
            else
            {
                var itemDirtyTracker = child as ItemDirtyTracker;
                if (itemDirtyTracker != null)
                {
                    if (!this.itemsTrackers.IsDirty)
                    {
                        this.diff.Remove(child.PropertyInfo);
                    }
                }
                else
                {
                    this.diff.Remove(child.PropertyInfo);
                }
            }

            this.NotifyChanges(before);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.propertiesTrackers?.Dispose();
                this.itemsTrackers?.Dispose();
            }
        }

        protected void NotifyChanges(int diffsBefore)
        {
            if (this.diff.Count != diffsBefore)
            {
                if ((diffsBefore == 0 && this.diff.Count > 0) || (diffsBefore > 0 && this.diff.Count == 0))
                {
                    this.OnPropertyChanged(nameof(this.IsDirty));
                }

                this.OnPropertyChanged(nameof(this.Diff));
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
