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

        public new bool Equals(object expected, object actual)
        {
            if (expected is null && actual is null)
            {
                return true;
            }

            if (expected is null && actual != null)
            {
                throw new AssertionException($"Expected actual to be null but was: {actual}");
            }

            if (expected != null && actual is null)
            {
                throw new AssertionException($"Expected actual to be {expected} but was: null");
            }

            if (expected.GetType() != actual.GetType())
            {
                throw new AssertionException($"Expected actual to be of type {expected.GetType().Name} but was: {actual.GetType().Name}");
            }

            if (TryCompare<PropertyChangeEventArgs>(expected, actual, PropertyChangedEventArgsComparer.Default.Equals, out bool result) ||
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

            throw new NotSupportedException("Handle: " + expected.GetType().Name);
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