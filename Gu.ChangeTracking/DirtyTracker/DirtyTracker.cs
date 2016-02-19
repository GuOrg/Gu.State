namespace Gu.ChangeTracking
{
    using System.ComponentModel;
    using System.Reflection;

    public static class DirtyTracker
    {
        public static DirtyTracker<T> Track<T>(T x, T y, ReferenceHandling referenceHandling)
            where T : class, INotifyPropertyChanged
        {
            return new DirtyTracker<T>(x, y, referenceHandling);
        }

        public static DirtyTracker<T> Track<T>(T x, T y, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            where T : class, INotifyPropertyChanged
        {
            return new DirtyTracker<T>(x, y, bindingFlags, referenceHandling);
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

        public static void Verify<T>(params string[] ignoreProperties)
            where T : class, INotifyPropertyChanged
        {
            DirtyTracker<T>.Verify(ignoreProperties);
        }

        public static void Verify<T>(BindingFlags bindingFlags, params string[] ignoreProperties)
            where T : class, INotifyPropertyChanged
        {
            DirtyTracker<T>.Verify(bindingFlags, ignoreProperties);
        }
    }
}