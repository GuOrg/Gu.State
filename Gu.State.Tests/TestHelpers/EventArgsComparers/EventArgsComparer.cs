#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
namespace Gu.State.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Is = Gu.State.Is;

    public sealed class EventArgsComparer : IEqualityComparer<object>, IComparer
    {
        public static readonly EventArgsComparer Default = new EventArgsComparer();

        private EventArgsComparer()
        {
        }

        public new bool Equals(object x, object y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null && y != null)
            {
                throw new AssertionException($"Expected y to be null but was: {y}");
            }

            if (x != null && y is null)
            {
                throw new AssertionException($"Expected y to be {x} but was: null");
            }

            if (x.GetType() != y.GetType())
            {
                throw new AssertionException($"Expected y to be of type {x.GetType().Name} but was: {y.GetType().Name}");
            }

            if (TryCompare<PropertyChangeEventArgs>(x, y, PropertyChangedEventArgsComparer.Default.Equals, out bool result) ||
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

            throw new NotSupportedException("Handle: " + x.GetType().Name);
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            throw new NotSupportedException();
        }

        int IComparer.Compare(object x, object y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is EventArgs xArgs &&
                y is EventArgs yArgs)
            {
                return this.Equals(xArgs, yArgs)
                    ? 0
                    : -1;
            }

            return -1;
        }

        private static bool TryCompare<T>(object x, object y, Func<T, T, bool> compare, out bool result)
        {
            if (Is.Type<T>(x, y))
            {
                result = compare((T)x, (T)y);
                return true;
            }

            result = false;
            return false;
        }
    }
}
