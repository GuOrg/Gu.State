namespace Gu.ChangeTracking
{
    using System.ComponentModel;
    using System.Reflection;

    public static partial class DirtyTracker
    {
        public static DirtyTracker<T> Track<T>(
            T x,
            T y,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw,
            params string[] ignoreProperties)
            where T : class, INotifyPropertyChanged
        {
            var settings = PropertiesSettings.Create(x, y, bindingFlags, referenceHandling, ignoreProperties);
            return new DirtyTracker<T>(x, y, settings);
        }

        public static DirtyTracker<T> Track<T>(T x, T y, PropertiesSettings settings)
            where T : class, INotifyPropertyChanged
        {
            return new DirtyTracker<T>(x, y, settings);
        }
    }
}