namespace Gu.ChangeTracking
{
    using System.ComponentModel;

    public static class DirtyTracker
    {
        public static DirtyTracker<T> Track<T>(T x, T y) 
            where T : class, INotifyPropertyChanged
        {
            return new DirtyTracker<T>(x, y);
        }
    }
}