namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
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
        private readonly HashSet<PropertyInfo> diff = new HashSet<PropertyInfo>();

        public DirtyTracker(T x, T y)
        {
            Ensure.NotNull(x, nameof(x));
            Ensure.NotNull(y, nameof(y));
            Ensure.SameType(x, y);
            Ensure.NotIs<IEnumerable>(x, nameof(x));
            Verify();
            this.x = x;
            this.y = y;
            Reset();
            x.PropertyChanged += OnTrackedPropertyChanged;
            y.PropertyChanged += OnTrackedPropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDirty => this.diff.Count != 0;

        public IEnumerable<PropertyInfo> Diff => this.diff;

        public static void Verify()
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                throw new NotSupportedException("Not supporting IEnumerable");
            }

            var notDiffable = typeof(T).GetProperties()
                .Where(p => !IsDiffableType(p.PropertyType))
                .ToArray();
            if (notDiffable.Any())
            {
                var sb = new StringBuilder();
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

        protected void Reset()
        {
            var before = this.diff.Count;
            this.diff.Clear();
            foreach (var prop in this.x.GetType().GetProperties())
            {
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

        private static bool IsDiffableType(Type type)
        {
            if (type == typeof(string))
            {
                return true;
            }

            return type.IsValueType && type.IsEquatable();
        }

        private void OnTrackedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName))
            {
                Reset();
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
