namespace Gu.ChangeTracking
{
    using System;
    using System.ComponentModel;

    public static class PropertySynchronizer
    {
        public static IDisposable Create<T>(T source, T destination, params string[] ignoreProperties)
            where T : class, INotifyPropertyChanged
        {
            return new PropertySynchronizer<T>(source, destination, ignoreProperties);
        }
    }
}