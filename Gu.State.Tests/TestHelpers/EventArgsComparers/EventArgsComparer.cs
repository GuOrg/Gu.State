namespace Gu.State.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class EventArgsComparer : IEqualityComparer<object>, IComparer
    {
        public static readonly EventArgsComparer Default = new EventArgsComparer();

        private EventArgsComparer()
        {
        }

        public new bool Equals(object x, object y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            bool result;
            if (TryCompare<PropertyChangeEventArgs>(x,y, PropertyChangedEventArgsComparer.Default.Equals, out result) ||
                TryCompare<AddEventArgs>(x, y, AddEventArgsComparer.Default.Equals, out result) ||
                TryCompare<RemoveEventArgs>(x, y, RemoveEventArgsComparer.Default.Equals, out result) ||
                TryCompare<ReplaceEventArgs>(x, y, ReplaceEventArgsComparer.Default.Equals, out result) ||
                TryCompare<MoveEventArgs>(x, y, MoveEventArgsComparer.Default.Equals, out result) ||
                TryCompare<ResetEventArgs>(x, y, ResetEventArgsComparer.Default.Equals, out result) ||
                TryCompare<RootChangeEventArgs<ChangeTrackerNode>>(x, y, RootChangeEventArgsEventArgsComparer<ChangeTrackerNode>.Default.Equals, out result) ||
                TryCompare<RootChangeEventArgs<DirtyTrackerNode>>(x, y, RootChangeEventArgsEventArgsComparer<DirtyTrackerNode>.Default.Equals, out result) ||
                TryCompare<PropertyGraphChangedEventArgs<ChangeTrackerNode>>(x, y, PropertyGraphChangedEventArgsComparer<ChangeTrackerNode>.Default.Equals, out result) ||
                TryCompare<PropertyGraphChangedEventArgs<DirtyTrackerNode>>(x, y, PropertyGraphChangedEventArgsComparer<DirtyTrackerNode>.Default.Equals, out result) ||
                TryCompare<ItemGraphChangedEventArgs<ChangeTrackerNode>>(x, y, ItemGraphChangedEventArgsComparer<ChangeTrackerNode>.Default.Equals, out result) ||
                TryCompare<ItemGraphChangedEventArgs<DirtyTrackerNode>>(x, y, ItemGraphChangedEventArgsComparer<DirtyTrackerNode>.Default.Equals, out result))
            {
                return result;
            }

            throw new NotImplementedException("Handle: " + x.GetType().Name);
        }

        private static bool TryCompare<T>(object x, object y, Func<T, T, bool> compare, out  bool result)
        {
            if (Is.Type<T>(x, y))
            {
                result = compare((T)x, (T)y);
                return true;
            }

            result = false;
            return false;
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        int IComparer.Compare(object x, object y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            var xArgs = x as EventArgs;
            var yArgs = y as EventArgs;
            if (xArgs == null || yArgs == null)
            {
                return -1;
            }

            return this.Equals(xArgs, yArgs)
                       ? 0
                       : -1;

        }
    }
}