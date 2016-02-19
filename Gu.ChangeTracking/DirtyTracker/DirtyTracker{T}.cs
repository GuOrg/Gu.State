namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    /// <summary>
    /// Tracks if there is a difference between the properties of x and y
    /// </summary>
    public class DirtyTracker<T> : INotifyPropertyChanged, IDisposable, IDirtyTrackerNode
        where T : class, INotifyPropertyChanged
    {
        private readonly T x;
        private readonly T y;
        private readonly HashSet<PropertyInfo> diff = new HashSet<PropertyInfo>();
        private readonly PropertyCollection propertyTrackers;
        private readonly ItemCollection<IDirtyTrackerNode> itemTrackers;

        internal DirtyTracker(T x, T y, ReferenceHandling referenceHandling)
            : this(x, y, Constants.DefaultPropertyBindingFlags, referenceHandling)
        {
        }

        internal DirtyTracker(T x, T y, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
                        : this(x, y, new DirtyTrackerSettings(null, bindingFlags, referenceHandling))
        {
        }

        public DirtyTracker(T x, T y, params string[] ignoreProperties)
            : this(x, y, Constants.DefaultPropertyBindingFlags, ignoreProperties)
        {
        }

        public DirtyTracker(T x, T y, BindingFlags bindingFlags, params string[] ignoreProperties)
            : this(x, y, new DirtyTrackerSettings(x?.GetType().GetIgnoreProperties(bindingFlags, ignoreProperties), bindingFlags, ReferenceHandling.Throw))
        {
        }

        public DirtyTracker(T x, T y, DirtyTrackerSettings settings)
            : this(x, y, settings, true)
        {
        }

        protected DirtyTracker(T x, T y, DirtyTrackerSettings settings, bool validateArguments)
        {
            Ensure.NotNull(x, nameof(x));
            Ensure.NotNull(y, nameof(y));
            if (validateArguments)
            {
                Ensure.NotSame(x, y, nameof(x), nameof(y));
            }

            Ensure.SameType(x, y);
            Verify(settings);
            this.x = x;
            this.y = y;
            this.Settings = settings;
            this.Reset();
            x.PropertyChanged += this.OnTrackedPropertyChanged;
            y.PropertyChanged += this.OnTrackedPropertyChanged;
            this.propertyTrackers = PropertyCollection.Create(x, y, settings, this.CreatePropertyTracker);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDirty => this.diff.Count != 0;

        PropertyInfo IDirtyTrackerNode.PropertyInfo => null;

        public IEnumerable<PropertyInfo> Diff => this.diff;

        public DirtyTrackerSettings Settings { get; }

        /// <summary>
        /// Check if <typeparamref name="T"/> can be tracked
        /// </summary>
        public static void Verify(params string[] ignoreProperties)
        {
            Verify(Constants.DefaultPropertyBindingFlags, ignoreProperties);
        }

        /// <summary>
        /// Check if <typeparamref name="T"/> can be tracked
        /// </summary>
        public static void Verify(BindingFlags bindingFlags, params string[] ignoreProperties)
        {
            Verify(new DirtyTrackerSettings(typeof(T).GetIgnoreProperties(bindingFlags, ignoreProperties), bindingFlags, ReferenceHandling.Throw));
        }

        public static void Verify(DirtyTrackerSettings settings)
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                if (settings.ReferenceHandling == ReferenceHandling.Throw ||
                    (settings.ReferenceHandling != ReferenceHandling.Throw && !typeof(INotifyCollectionChanged).IsAssignableFrom(typeof(T))))
                {
                    throw new NotSupportedException("Not supporting IEnumerable unless ReferenceHandling is specified and the collection is INotifyCollectionChanged");
                }
            }

            foreach (var propertyInfo in typeof(T).GetProperties(settings.BindingFlags))
            {
                if (settings.IsIgnoringProperty(propertyInfo))
                {
                    continue;
                }

                if (!EqualBy.IsEquatable(propertyInfo.PropertyType) && settings.ReferenceHandling == ReferenceHandling.Throw)
                {
                    var message = $"Only equatable properties are supported without specifying {typeof(ReferenceHandling).Name}\r\n" +
                                  $"Property {typeof(T).Name}.{propertyInfo.Name} is not IEquatable<{propertyInfo.PropertyType.Name}>.\r\n" +
                                  "Use the overload DirtyTracker.Track(x, y, ReferenceHandling) if you want to track a graph";
                    throw new NotSupportedException(message);
                }
            }
        }

        public void Dispose()
        {
            this.x.PropertyChanged -= this.OnTrackedPropertyChanged;
            this.y.PropertyChanged -= this.OnTrackedPropertyChanged;
            this.propertyTrackers?.Dispose();
            this.itemTrackers?.Dispose();
        }

        void IDirtyTrackerNode.Update(IDirtyTrackerNode child)
        {
            var before = this.diff.Count;
            if (child.IsDirty)
            {
                this.diff.Add(child.PropertyInfo);
            }
            else
            {
                this.diff.Remove(child.PropertyInfo);
            }

            this.NotifyChanges(before);
        }

        /// <summary>
        /// Clears the <see cref="Diff"/> and calculates a new.
        /// Notifies if there are changes.
        /// </summary>
        protected void Reset()
        {
            var before = this.diff.Count;
            this.diff.Clear();
            foreach (var propertyInfo in this.x.GetType().GetProperties(this.Settings.BindingFlags))
            {
                if (this.Settings.IsIgnoringProperty(propertyInfo))
                {
                    continue;
                }

                var xv = propertyInfo.GetValue(this.x);
                var yv = propertyInfo.GetValue(this.y);
                if (this.propertyTrackers != null && this.propertyTrackers.Contains(propertyInfo))
                {
                    var propertyTracker = this.CreatePropertyTracker((INotifyPropertyChanged)xv, (INotifyPropertyChanged)yv, propertyInfo);
                    this.propertyTrackers[propertyInfo] = propertyTracker;
                }
                else if (!Equals(xv, yv))
                {
                    this.diff.Add(propertyInfo);
                }
            }

            this.NotifyChanges(before);
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

        private void OnTrackedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName))
            {
                this.Reset();
                return;
            }

            var propertyInfo = sender.GetType().GetProperty(e.PropertyName, this.Settings.BindingFlags);
            if (propertyInfo == null)
            {
                return;
            }

            if (this.Settings.IsIgnoringProperty(propertyInfo))
            {
                return;
            }

            var xv = propertyInfo.GetValue(this.x);
            var yv = propertyInfo.GetValue(this.y);
            if (this.propertyTrackers != null && this.propertyTrackers.Contains(propertyInfo))
            {
                var propertyTracker = this.CreatePropertyTracker((INotifyPropertyChanged)xv, (INotifyPropertyChanged)yv, propertyInfo);
                this.propertyTrackers[propertyInfo] = propertyTracker;
                ((IDirtyTrackerNode)this).Update(propertyTracker);
                return;
            }

            var before = this.diff.Count;
            if (!Equals(xv, yv))
            {
                this.diff.Add(propertyInfo);
            }
            else
            {
                this.diff.Remove(propertyInfo);
            }

            this.NotifyChanges(before);
        }

        private IDirtyTrackerNode CreatePropertyTracker(INotifyPropertyChanged x, INotifyPropertyChanged y, PropertyInfo propertyInfo)
        {
            if (x == null && y == null)
            {
                return new NeverDirtyNode(propertyInfo);
            }

            if (x == null || y == null)
            {
                return new AlwaysDirtyNode(propertyInfo);
            }

            return new PropertyDirtyTracker(x, y, this, propertyInfo, this.Settings);
        }
    }
}
