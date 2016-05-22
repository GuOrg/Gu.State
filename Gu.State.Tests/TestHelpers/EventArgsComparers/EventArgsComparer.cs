namespace Gu.State.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Is = Gu.State.Is;

    public class EventArgsComparer : IEqualityComparer<object>, IComparer
    {
        public static readonly EventArgsComparer Default = new EventArgsComparer();

        private EventArgsComparer()
        {
        }

        public new bool Equals(object expected, object actual)
        {
            if (expected == null && actual == null)
            {
                return true;
            }

            if (expected == null && actual != null)
            {
                throw new AssertionException($"Expected actual to be null but was: {actual}");
            }

            if (expected != null && actual == null)
            {
                throw new AssertionException($"Expected actual to be {expected} but was: null");
            }

            if (expected.GetType() != actual.GetType())
            {
                throw new AssertionException($"Expected actual to be of type {expected.GetType().Name} but was: {actual.GetType().Name}");
            }

            bool result;
            if (TryCompare<PropertyChangeEventArgs>(expected,actual, PropertyChangedEventArgsComparer.Default.Equals, out result) ||
                TryCompare<AddEventArgs>(expected, actual, AddEventArgsComparer.Default.Equals, out result) ||
                TryCompare<RemoveEventArgs>(expected, actual, RemoveEventArgsComparer.Default.Equals, out result) ||
                TryCompare<ReplaceEventArgs>(expected, actual, ReplaceEventArgsComparer.Default.Equals, out result) ||
                TryCompare<MoveEventArgs>(expected, actual, MoveEventArgsComparer.Default.Equals, out result) ||
                TryCompare<ResetEventArgs>(expected, actual, ResetEventArgsComparer.Default.Equals, out result) ||
                TryCompare<RootChangeEventArgs<ChangeTrackerNode>>(expected, actual, RootChangeEventArgsEventArgsComparer<ChangeTrackerNode>.Default.Equals, out result) ||
                TryCompare<RootChangeEventArgs<DirtyTrackerNode>>(expected, actual, RootChangeEventArgsEventArgsComparer<DirtyTrackerNode>.Default.Equals, out result) ||
                TryCompare<PropertyGraphChangedEventArgs<ChangeTrackerNode>>(expected, actual, PropertyGraphChangedEventArgsComparer<ChangeTrackerNode>.Default.Equals, out result) ||
                TryCompare<PropertyGraphChangedEventArgs<DirtyTrackerNode>>(expected, actual, PropertyGraphChangedEventArgsComparer<DirtyTrackerNode>.Default.Equals, out result) ||
                TryCompare<ItemGraphChangedEventArgs<ChangeTrackerNode>>(expected, actual, ItemGraphChangedEventArgsComparer<ChangeTrackerNode>.Default.Equals, out result) ||
                TryCompare<ItemGraphChangedEventArgs<DirtyTrackerNode>>(expected, actual, ItemGraphChangedEventArgsComparer<DirtyTrackerNode>.Default.Equals, out result))
            {
                return result;
            }

            throw new NotImplementedException("Handle: " + expected.GetType().Name);
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