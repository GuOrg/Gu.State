namespace Gu.State
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    public static partial class DiffBy
    {
        internal static void UpdateDiffs<T>(this DiffBuilder builder, T x, T y, MemberSettings settings)
        {
            EqualBy.Verify.CanEqualByMemberValues(x, y, settings, typeof(DiffBy).Name, settings.DiffMethodName());
            builder.TryAddCollectionDiffs(x, y, settings);
            TryAddMemberDiffs(x, y, settings, builder);
        }

        internal static void UpdateMemberDiff(
            this DiffBuilder builder,
            object xSource,
            object ySource,
            MemberInfo member,
            MemberSettings settings)
        {
            if (settings.IsIgnoringMember(member))
            {
                return;
            }

            var getterAndSetter = settings.GetOrCreateGetterAndSetter(member);
            if (getterAndSetter.TryGetValueEquals(xSource, ySource, settings, out var equal, out var xValue, out var yValue))
            {
                if (equal)
                {
                    builder.Remove(member);
                }
                else
                {
                    builder.TryAdd(member, xValue, yValue);
                }

                return;
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.References when xValue?.GetType().IsValueType == true &&
                                                       yValue?.GetType().IsValueType == true:
                    goto case ReferenceHandling.Structural;

                case ReferenceHandling.References:
                    if (ReferenceEquals(xValue, yValue))
                    {
                        builder.Remove(member);
                    }
                    else
                    {
                        builder.TryAdd(member, xValue, yValue);
                    }

                    return;
                case ReferenceHandling.Structural:
                    if (DiffBuilder.TryCreate(xValue, yValue, settings, out var subDiffBuilder))
                    {
                        subDiffBuilder.Value.UpdateDiffs(xValue, yValue, settings);
                    }

                    builder.AddLazy(member, subDiffBuilder.Value);
                    return;
                case ReferenceHandling.Throw:
                    throw Throw.ShouldNeverGetHereException();
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(settings.ReferenceHandling),
                        settings.ReferenceHandling,
                        null);
            }
        }

        internal static void UpdateCollectionItemDiff(
            this DiffBuilder collectionBuilder,
            object xItem,
            object yItem,
            object index,
            MemberSettings settings)
        {
            if (TryGetValueDiff(xItem, yItem, settings, out var diff))
            {
                if (diff != null)
                {
                    collectionBuilder.Add(new IndexDiff(index, diff));
                }
                else
                {
                    collectionBuilder.Remove(index);
                }

                return;
            }

            if (settings.ReferenceHandling == ReferenceHandling.References)
            {
                if (ReferenceEquals(xItem, yItem))
                {
                    collectionBuilder.Remove(index);
                }
                else
                {
                    collectionBuilder.Add(new IndexDiff(index, new ValueDiff(xItem, yItem)));
                }

                return;
            }

            if (DiffBuilder.TryCreate(xItem, yItem, settings, out var subDiffBuilder))
            {
                subDiffBuilder.Value.UpdateDiffs(xItem, yItem, settings);
            }

            collectionBuilder.AddLazy(index, subDiffBuilder.Value);
        }

        private static ValueDiff TryCreateValueDiff<T>(T x, T y, MemberSettings settings)
        {
            Debug.Assert(x != null, "x == null");
            Debug.Assert(y != null, "y == null");
            Debug.Assert(settings != null, "settings == null");

            if (TryGetValueDiff(x, y, settings, out var diff))
            {
                return diff;
            }

            using (var borrow = DiffBuilder.GetOrCreate(x, y, settings))
            {
                borrow.Value.UpdateDiffs(x, y, settings);
                return borrow.Value.CreateValueDiffOrNull();
            }
        }

        private static void TryAddMemberDiffs(
            object x,
            object y,
            MemberSettings settings,
            DiffBuilder builder)
        {
            foreach (var member in settings.GetMembers(x.GetType()))
            {
                builder.UpdateMemberDiff(x, y, member, settings);
            }
        }

        private static void TryAddCollectionDiffs(
            this DiffBuilder collectionBuilder,
            object x,
            object y,
            MemberSettings settings)
        {
            if (!Is.Enumerable(x, y))
            {
                return;
            }

            if (ListDiffBy.TryGetOrCreate(x, y, out var comparer) ||
                ReadonlyListDiffBy.TryGetOrCreate(x, y, out comparer) ||
                ArrayDiffBy.TryGetOrCreate(x, y, out comparer) ||
                DictionaryDiffBy.TryGetOrCreate(x, y, out comparer) ||
                ReadOnlyDictionaryDiffBy.TryGetOrCreate(x, y, out comparer) ||
                SetDiffBy.TryGetOrCreate(x, y, out comparer) ||
                EnumerableDiffBy.TryGetOrCreate(x, y, out comparer))
            {
                comparer.AddDiffs(collectionBuilder, x, y, settings);
                return;
            }

            throw Throw.ShouldNeverGetHereException("All enumarebles must be checked here");
        }
    }
}