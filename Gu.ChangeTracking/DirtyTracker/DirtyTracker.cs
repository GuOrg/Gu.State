namespace Gu.ChangeTracking
{
    using System.ComponentModel;
    using System.Reflection;

    public static partial class DirtyTracker
    {
        public static DirtyTracker<T> Track<T>(T x, T y, BindingFlags bindingFlags)
            where T : class, INotifyPropertyChanged
        {
            var settings = DirtyTrackerSettings.GetOrCreate(bindingFlags);
            return new DirtyTracker<T>(x, y, settings);
        }

        public static DirtyTracker<T> Track<T>(T x, T y, ReferenceHandling referenceHandling)
            where T : class, INotifyPropertyChanged
        {
            var settings = DirtyTrackerSettings.GetOrCreate(referenceHandling);
            return new DirtyTracker<T>(x, y, settings);
        }

        public static DirtyTracker<T> Track<T>(T x, T y, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            where T : class, INotifyPropertyChanged
        {
            var settings = DirtyTrackerSettings.GetOrCreate(bindingFlags, referenceHandling);
            return new DirtyTracker<T>(x, y, settings);
        }

        public static DirtyTracker<T> Track<T>(T x, T y, params string[] ignoreProperties)
            where T : class, INotifyPropertyChanged
        {
            return new DirtyTracker<T>(x, y, ignoreProperties);
        }

        public static DirtyTracker<T> Track<T>(T x, T y, BindingFlags bindingFlags, params string[] ignoreProperties)
            where T : class, INotifyPropertyChanged
        {
            return new DirtyTracker<T>(x, y, bindingFlags, ignoreProperties);
        }
    }
}