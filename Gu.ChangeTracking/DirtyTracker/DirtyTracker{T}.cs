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
    public class DirtyTracker<T> : INotifyPropertyChanged, IDisposable
        where T : class, INotifyPropertyChanged
    {
        private readonly T x;
        private readonly T y;
        private readonly HashSet<PropertyInfo> diff = new HashSet<PropertyInfo>();

        public DirtyTracker(T x, T y, ReferenceHandling referenceHandling)
            : this(x, y, Constants.DefaultPropertyBindingFlags, referenceHandling)
        {
        }

        public DirtyTracker(T x, T y, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
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
        {
            Ensure.NotNull(x, nameof(x));
            Ensure.NotNull(y, nameof(y));
            Ensure.NotSame(x, y, nameof(x), nameof(y));
            Ensure.SameType(x, y);
            Verify(settings);
            this.x = x;
            this.y = y;
            this.Settings = settings;
            this.Reset();
            x.PropertyChanged += this.OnTrackedPropertyChanged;
            y.PropertyChanged += this.OnTrackedPropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDirty => this.diff.Count != 0;

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
        }

        /// <summary>
        /// Clears the <see cref="Diff"/> and calculates a new.
        /// Notifies if there are changes.
        /// </summary>
        protected void Reset()
        {
            var before = this.diff.Count;
            this.diff.Clear();
            foreach (var prop in this.x.GetType().GetProperties(this.Settings.BindingFlags))
            {
                if (this.Settings.IsIgnoringProperty(prop))
                {
                    continue;
                }

                var xv = prop.GetValue(this.x);
                var yv = prop.GetValue(this.y);
                if (!Equals(xv, yv))
                {
                    this.diff.Add(prop);
                }
            }

            if (this.diff.Count != before)
            {
                this.OnPropertyChanged(nameof(this.IsDirty));
                this.OnPropertyChanged(nameof(this.Diff));
            }
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
            var before = this.diff.Count;
            if (!Equals(xv, yv))
            {
                if (this.diff.Add(propertyInfo))
                {
                    if (before == 0)
                    {
                        this.OnPropertyChanged(nameof(this.IsDirty));
                    }

                    this.OnPropertyChanged(nameof(this.Diff));
                }
            }
            else
            {
                if (this.diff.Remove(propertyInfo))
                {
                    if (before == 1)
                    {
                        this.OnPropertyChanged(nameof(this.IsDirty));
                    }

                    this.OnPropertyChanged(nameof(this.Diff));
                }
            }
        }
    }
}
