namespace Gu.State
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    public static partial class DiffBy
    {
        private static class MemberValues
        {
            internal static ValueDiff Diffs<T>(T x, T y, IMemberSettings settings)
            {
                Debug.Assert(x != null, "x == null");
                Debug.Assert(y != null, "y == null");
                Debug.Assert(settings != null, "settings == null");

                ValueDiff diff;
                if (TryGetValueDiff(x, y, settings, out diff))
                {
                    return diff;
                }

                EqualBy.Verify.CanEqualByMemberValues(x, y, settings, typeof(DiffBy).Name, settings.DiffMethodName());
                using (var borrow = DiffBuilder.Create(x, y))
                {
                    TryAddDiffs(x, y, settings, borrow.Value);
                    return borrow.Value.CreateValueDiff();
                }
            }

            private static void TryAddDiffs<T>(
                T x,
                T y,
                IMemberSettings settings,
                DiffBuilder builder)
            {
                Enumerable.AddItemDiffs(x, y, settings, builder, TryAddItemDiff);
                foreach (var member in settings.GetMembers(x.GetType()))
                {
                    TryAddMemberDiff(x, y, member, settings, builder);
                }
            }

            private static void TryAddItemDiff(
                object xItem,
                object yItem,
                object index,
                IMemberSettings settings,
                DiffBuilder collectionBuilder)
            {
                ValueDiff diff;
                if (TryGetValueDiff(xItem, yItem, settings, out diff))
                {
                    if (diff != null)
                    {
                        collectionBuilder.Add(new IndexDiff(index, diff));
                    }

                    return;
                }

                if (settings.ReferenceHandling == ReferenceHandling.References)
                {
                    if (!ReferenceEquals(xItem, yItem))
                    {
                        collectionBuilder.Add(new IndexDiff(index, new ValueDiff(xItem, yItem)));
                    }

                    return;
                }

                EqualBy.Verify.CanEqualByMemberValues(xItem, yItem, settings, typeof(DiffBy).Name, settings.DiffMethodName());
                DiffBuilder subDiffBuilder;
                if (collectionBuilder.TryAdd(xItem, yItem, out subDiffBuilder))
                {
                    TryAddDiffs(xItem, yItem, settings, subDiffBuilder);
                }

                collectionBuilder.AddLazy(index, subDiffBuilder);
            }

            private static void TryAddMemberDiff(
                object xSource,
                object ySource,
                MemberInfo member,
                IMemberSettings settings,
                DiffBuilder builder)
            {
                if (settings.IsIgnoringMember(member))
                {
                    return;
                }

                var getterAndSetter = settings.GetOrCreateGetterAndSetter(member);
                bool equal;
                object xValue;
                object yValue;
                if (getterAndSetter.TryGetValueEquals(xSource, ySource, settings, out equal, out xValue, out yValue))
                {
                    if (!equal)
                    {
                        builder.Add(State.MemberDiff.Create(member, xValue, yValue));
                    }

                    return;
                }

                switch (settings.ReferenceHandling)
                {
                    case ReferenceHandling.References:
                        if (!ReferenceEquals(xValue, yValue))
                        {
                            builder.Add(State.MemberDiff.Create(member, new ValueDiff(xValue, yValue)));
                        }

                        return;
                    case ReferenceHandling.Structural:
                    case ReferenceHandling.StructuralWithReferenceLoops:
                        EqualBy.Verify.CanEqualByMemberValues(xValue, yValue, settings, typeof(DiffBy).Name, settings.DiffMethodName());
                        DiffBuilder subDiffBuilder;
                        if (builder.TryAdd(xValue, yValue, out subDiffBuilder))
                        {
                            TryAddDiffs(xValue, yValue, settings, subDiffBuilder);
                        }

                        builder.AddLazy(member, subDiffBuilder);
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
        }
    }
}