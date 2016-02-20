namespace Gu.ChangeTracking
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    public static class PropertySynchronizer
    {
        public static IDisposable Create<T>(T source, T destination, BindingFlags bindingFlags)
            where T : class, INotifyPropertyChanged
        {
            var settings = CopyPropertiesSettings.GetOrCreate(bindingFlags, ReferenceHandling.Throw);
            return new PropertySynchronizer<T>(source, destination, settings);
        }

        public static IDisposable Create<T>(T source, T destination, ReferenceHandling referenceHandling)
            where T : class, INotifyPropertyChanged
        {
            var settings = CopyPropertiesSettings.GetOrCreate(Constants.DefaultPropertyBindingFlags, referenceHandling);
            return new PropertySynchronizer<T>(source, destination, settings);
        }

        public static IDisposable Create<T>(T source, T destination, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            where T : class, INotifyPropertyChanged
        {
            var settings = CopyPropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
            return new PropertySynchronizer<T>(source, destination, settings);
        }

        public static IDisposable Create<T>(T source, T destination, params string[] excludedProperties)
            where T : class, INotifyPropertyChanged
        {
            return new PropertySynchronizer<T>(source, destination, excludedProperties);
        }

        public static IDisposable Create<T>(T source, T destination, BindingFlags bindingFlags, params string[] excludedProperties)
            where T : class, INotifyPropertyChanged
        {
            return new PropertySynchronizer<T>(source, destination, bindingFlags, excludedProperties);
        }

        public static IDisposable Create<T>(T source, T target, CopyPropertiesSettings settings)
            where T : class, INotifyPropertyChanged
        {
            return new PropertySynchronizer<T>(source, target, settings);
        }

        public static void VerifyCanSyncronize<T>(params string[] excludedProperties)
        {
            Copy.VerifyCanCopyPropertyValues<T>(excludedProperties);
        }

        public static void VerifyCanSyncronize<T>(BindingFlags bindingFlags, params string[] excludedProperties)
        {
            Copy.VerifyCanCopyPropertyValues<T>(bindingFlags, excludedProperties);
        }
    }
}