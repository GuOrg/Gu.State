namespace Gu.ChangeTracking
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    public static class PropertySynchronizer
    {
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