namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal class SetDiffBy<T> : SetDiffBy, IDiffBy
    {
        public static readonly SetDiffBy<T> Default = new SetDiffBy<T>();

        private SetDiffBy()
        {
        }

        public void AddDiffs<TSettings>(
            object x,
            object y,
            TSettings settings,
            DiffBuilder collectionBuilder,
            Action<object, object, object, TSettings, DiffBuilder> itemDiff)
            where TSettings : IMemberSettings
        {
            this.AddDiffs((ISet<T>)x, (ISet<T>)y, settings, collectionBuilder, itemDiff);
        }

        private static void GetItemDiffs(ISet<T> x, ISet<T> y, DiffBuilder collectionBuilder, Disposer<HashSet<T>> borrow)
        {
            borrow.Value.UnionWith(x);
            if (borrow.Value.SetEquals(y))
            {
                return;
            }

            borrow.Value.ExceptWith(y);
            foreach (var xi in borrow.Value)
            {
                collectionBuilder.Add(new IndexDiff(xi, new ValueDiff(xi, PaddedPairs.MissingItem)));
            }

            borrow.Value.Clear();
            borrow.Value.UnionWith(y);
            borrow.Value.ExceptWith(x);
            foreach (var yi in borrow.Value)
            {
                collectionBuilder.Add(new IndexDiff(yi, new ValueDiff(PaddedPairs.MissingItem, yi)));
            }
        }

        private void AddDiffs<TSettings>(
            ISet<T> x,
            ISet<T> y,
            TSettings settings,
            DiffBuilder collectionBuilder,
            Action<object, object, object, TSettings, DiffBuilder> itemDiff)
                where TSettings : IMemberSettings
        {
            if (typeof(T).Implements<IEquatable<T>>())
            {
                using (var borrow = SetPool<T>.Borrow(EqualityComparer<T>.Default.Equals, EqualityComparer<T>.Default.GetHashCode))
                {
                    GetItemDiffs(x, y, collectionBuilder, borrow);
                    return;
                }
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.Throw:
                    throw Throw.ShouldNeverGetHereException("ReferenceHandling should be checked before");
                case ReferenceHandling.References:
                    using (var borrow = SetPool<T>.Borrow((xi, yi) => ReferenceEquals(xi, yi), item => RuntimeHelpers.GetHashCode(item)))
                    {
                        GetItemDiffs(x, y, collectionBuilder, borrow);
                        return;
                    }

                case ReferenceHandling.Structural:
                case ReferenceHandling.StructuralWithReferenceLoops:
                    using (var borrow = SetPool<T>.Borrow((xi, yi) => EqualBy.MemberValues(xi, yi, settings), xi => 0))
                    {
                        GetItemDiffs(x, y, collectionBuilder, borrow);
                        return;
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}