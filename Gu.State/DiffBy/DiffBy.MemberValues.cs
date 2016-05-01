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
                var builder = new DiffBuilderRoot(x, y, settings.ReferenceHandling);
                AddSubDiffs(x, y, settings, builder);
                return builder.CreateValueDiff();
            }

            private static void AddSubDiffs<T>(
                T x,
                T y,
                IMemberSettings settings,
                DiffBuilder builder)
            {
                Enumerable.AddItemDiffs(x, y, settings, builder, ItemMembersDiff);
                foreach (var member in settings.GetMembers(x.GetType()))
                {
                    if (settings.IsIgnoringMember(member))
                    {
                        continue;
                    }

                    var getterAndSetter = settings.GetOrCreateGetterAndSetter(member);
                    if (settings.IsEquatable(getterAndSetter.ValueType))
                    {
                        if (!getterAndSetter.ValueEquals(x, y))
                        {
                            builder.Add(MemberDiff.Create(member, getterAndSetter.GetValue(x), getterAndSetter.GetValue(y)));
                        }

                        continue;
                    }

                    var xv = getterAndSetter.GetValue(x);
                    var yv = getterAndSetter.GetValue(y);
                    MemberValueDiff(xv, yv, member, settings, builder);
                }
            }

            private static void ItemMembersDiff(
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
                    AddSubDiffs(xItem, yItem, settings, subDiffBuilder);
                }

                collectionBuilder.AddLazy(index, subDiffBuilder);
            }

            private static void MemberValueDiff(
                object xValue,
                object yValue,
                MemberInfo member,
                IMemberSettings settings,
                DiffBuilder builder)
            {
                ValueDiff diff;
                if (TryGetValueDiff(xValue, yValue, settings, out diff))
                {
                    if (diff != null)
                    {
                        builder.Add(MemberDiff.Create(member, diff));
                    }

                    return;
                }

                switch (settings.ReferenceHandling)
                {
                    case ReferenceHandling.References:
                        if (!ReferenceEquals(xValue, yValue))
                        {
                            builder.Add(MemberDiff.Create(member, new ValueDiff(xValue, yValue)));
                        }

                        return;
                    case ReferenceHandling.Structural:
                    case ReferenceHandling.StructuralWithReferenceLoops:
                        EqualBy.Verify.CanEqualByMemberValues(xValue, yValue, settings, typeof(DiffBy).Name, settings.DiffMethodName());
                        DiffBuilder subDiffBuilder;
                        if (builder.TryAdd(xValue, yValue, out subDiffBuilder))
                        {
                            AddSubDiffs(xValue, yValue, settings, subDiffBuilder);
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