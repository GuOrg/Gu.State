namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using JetBrains.Annotations;

    /// <summary>
    /// Tracks if there is a difference between the properties of x and y
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DirtyTracker<T> : INotifyPropertyChanged, IDisposable
        where T : class, INotifyPropertyChanged
    {
        private readonly T x;
        private readonly T y;
        private readonly string[] ignoreProperties;
        private readonly HashSet<PropertyInfo> diff = new HashSet<PropertyInfo>();

        public DirtyTracker(T x, T y, params string[] ignoreProperties)
        {
            Ensure.NotNull(x, nameof(x));
            Ensure.NotNull(y, nameof(y));
            Ensure.SameType(x, y);
            Ensure.NotIs<IEnumerable>(x, nameof(x));
            Verify(ignoreProperties);
            this.x = x;
            this.y = y;
            this.ignoreProperties = ignoreProperties;
            Reset();
            x.PropertyChanged += OnTrackedPropertyChanged;
            y.PropertyChanged += OnTrackedPropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDirty => this.diff.Count != 0;

        public IEnumerable<PropertyInfo> Diff => this.diff;

        /// <summary>
        /// Check if <typeparamref name="T"/> can be tracked
        /// </summary>
        public static void Verify(params string[] ignoreProperties)
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                throw new NotSupportedException("Not supporting IEnumerable");
            }

            var notDiffable = typeof(T).GetProperties()
                .Where(p => !ignoreProperties?.Contains(p.Name) == true)
                .Where(p => !IsDiffable(p))
                .ToArray();
            if (notDiffable.Any())
            {
                var sb = new StringBuilder();
                sb.AppendLine("Only supports simple properties like string & int");
                foreach (var prop in notDiffable)
                {
                    sb.AppendLine($"Property {prop} is not diffable.");
                }

                throw new NotSupportedException(sb.ToString());
            }
        }

        public void Dispose()
        {
            this.x.PropertyChanged -= OnTrackedPropertyChanged;
            this.y.PropertyChanged -= OnTrackedPropertyChanged;
        }

        /// <summary>
        /// Clears the <see cref="Diff"/> and calculates a new.
        /// Notifies if there are changes.
        /// </summary>
        protected void Reset()
        {
            var before = this.diff.Count;
            this.diff.Clear();
            foreach (var prop in this.x.GetType().GetProperties())
            {
                if (this.ignoreProperties?.Contains(prop.Name) == true)
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
                OnPropertyChanged(nameof(IsDirty));
                OnPropertyChanged(nameof(Diff));
            }
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static bool IsDiffable(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType == typeof(string))
            {
                return true;
            }

            return propertyInfo.PropertyType.IsValueType && propertyInfo.PropertyType.IsEquatable();
        }

        private void OnTrackedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName))
            {
                Reset();
                return;
            }

            if (this.ignoreProperties?.Contains(e.PropertyName) == true)
            {
                return;
            }

            var prop = this.x.GetType().GetProperty(e.PropertyName);
            if (prop == null)
            {
                return;
            }

            var xv = prop.GetValue(this.x);
            var yv = prop.GetValue(this.y);
            var before = this.diff.Count;
            if (!Equals(xv, yv))
            {
                if (this.diff.Add(prop))
                {
                    if (before == 0)
                    {
                        OnPropertyChanged(nameof(IsDirty));
                    }

                    OnPropertyChanged(nameof(Diff));
                }
            }
            else
            {
                if (this.diff.Remove(prop))
                {
                    if (before == 1)
                    {
                        OnPropertyChanged(nameof(IsDirty));
                    }

                    OnPropertyChanged(nameof(Diff));
                }
            }
        }
    }
}
