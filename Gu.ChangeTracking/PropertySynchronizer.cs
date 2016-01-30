namespace Gu.ChangeTracking
{
    using System;
    using System.ComponentModel;

    public static class PropertySynchronizer
    {
        public static IDisposable Create<T>(T source, T destination, params string[] excludedProperties)
            where T : class, INotifyPropertyChanged
        {
            return new PropertySynchronizer<T>(source, destination, excludedProperties);
        }

        public static void VerifyCanSyncronize<T>( params string[] excludedProperties)
        {
            Copy.VerifyCanCopyPropertyValues<T>(excludedProperties);
        }
    }
}