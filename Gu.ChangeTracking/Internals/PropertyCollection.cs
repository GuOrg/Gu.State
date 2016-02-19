namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    internal class PropertyCollection
    {
        private readonly IReadOnlyList<PropertyAndDisposable> items;

        private PropertyCollection(List<PropertyAndDisposable> items)
        {
            this.items = items;
        }

        public bool Contains(PropertyInfo propertyInfo)
        {
            foreach (var item in this.items)
            {
                if (item.PropertyInfo == propertyInfo)
                {
                    return true;
                }
            }

            return false;
        }

        internal IDisposable this[PropertyInfo index]
        {
            get
            {
                return this.items.Single(x => x.PropertyInfo == index).Synchronizer;
            }
            set
            {
                this.items.Single(x => x.PropertyInfo == index)
                    .Synchronizer = value;
            }
        }

        public void Dispose()
        {
            if (this.items == null)
            {
                return;
            }

            foreach (var item in this.items)
            {
                item?.Dispose();
            }
        }

        internal static PropertyCollection Create(
            INotifyPropertyChanged source,
            INotifyPropertyChanged target,
            CopyPropertiesSettings settings,
            Func<INotifyPropertyChanged, INotifyPropertyChanged, IDisposable> createSynchronizer)
        {
            List<PropertyAndDisposable> items = null;
            foreach (var propertyInfo in source.GetType()
                                               .GetProperties(settings.BindingFlags))
            {
                if (settings.IsIgnoringProperty(propertyInfo) || settings.GetSpecialCopyProperty(propertyInfo) != null)
                {
                    continue;
                }

                if (!Copy.IsCopyableType(propertyInfo.PropertyType))
                {
                    var sv = propertyInfo.GetValue(source);
                    var tv = propertyInfo.GetValue(target);
                    var synchronizer = createSynchronizer((INotifyPropertyChanged)sv, (INotifyPropertyChanged)tv);
                    if (items == null)
                    {
                        items = new List<PropertyAndDisposable>();
                    }

                    items.Add(new PropertyAndDisposable(propertyInfo, synchronizer));
                }
            }

            if (items == null)
            {
                return null;
            }

            return new PropertyCollection(items);
        }

        internal static PropertyCollection Create<T>(
            T x,
            T y,
            DirtyTrackerSettings settings,
            Func<INotifyPropertyChanged, INotifyPropertyChanged, PropertyInfo, IDisposable> createTracker)
            where T : class, INotifyPropertyChanged
        {
            List<PropertyAndDisposable> items = null;
            foreach (var propertyInfo in x.GetType()
                                          .GetProperties(settings.BindingFlags))
            {
                if (settings.IsIgnoringProperty(propertyInfo))
                {
                    continue;
                }

                if (!Copy.IsCopyableType(propertyInfo.PropertyType))
                {
                    var sv = propertyInfo.GetValue(x);
                    var tv = propertyInfo.GetValue(y);
                    if (items == null)
                    {
                        items = new List<PropertyAndDisposable>();
                    }

                    var tracker = createTracker((INotifyPropertyChanged)sv, (INotifyPropertyChanged)tv, propertyInfo);
                    items.Add(new PropertyAndDisposable(propertyInfo, tracker));
                }
            }

            if (items == null)
            {
                return null;
            }

            return new PropertyCollection(items);
        }

        private class PropertyAndDisposable : IDisposable
        {
            private IDisposable synchronizer;

            public PropertyAndDisposable(PropertyInfo propertyInfo, IDisposable synchronizer)
            {
                this.PropertyInfo = propertyInfo;
                this.synchronizer = synchronizer;
            }

            public PropertyInfo PropertyInfo { get; }

            public IDisposable Synchronizer
            {
                get
                {
                    return this.synchronizer;
                }
                set
                {
                    this.synchronizer?.Dispose();
                    this.synchronizer = value;
                }
            }

            public void Dispose()
            {
                this.Synchronizer?.Dispose();
            }
        }
    }
}