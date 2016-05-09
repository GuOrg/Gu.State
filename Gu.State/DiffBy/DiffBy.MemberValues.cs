namespace Gu.State
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    public static partial class DiffBy
    {
        internal static void UpdateDiffs<T>(
           this DiffBuilder builder,
            T x,
            T y,
            IMemberSettings settings)
        {
            EqualBy.Verify.CanEqualByMemberValues(x, y, settings, typeof(DiffBy).Name, settings.DiffMethodName());
            TryAddCollectionDiffs(x, y, settings, builder, UpdateIndexDiff);
            TryAddMemberDiffs(x, y, settings, builder);
        }

        internal static void UpdateMemberDiff(
            this DiffBuilder builder,
            object xSource,
            object ySource,
            MemberInfo member,
            IMemberSettings settings)
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
                if (equal)
                {
                    builder.Remove(member);
                }
                else
                {
                    builder.Add(State.MemberDiff.Create(member, xValue, yValue));
                }

                return;
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.References:
                    if (ReferenceEquals(xValue, yValue))
                    {
                        builder.Remove(member);
                    }
                    else
                    {
                        builder.Add(State.MemberDiff.Create(member, new ValueDiff(xValue, yValue)));
                    }

                    return;
                case ReferenceHandling.Structural:
                case ReferenceHandling.StructuralWithReferenceLoops:
                    IRefCounted<DiffBuilder> subDiffBuilder;
                    if (DiffBuilder.TryCreate(xValue, yValue, settings, out subDiffBuilder))
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

        internal static void UpdateIndexDiff(
            this DiffBuilder collectionBuilder,
            object xItem,
            object yItem,
            object index,
            IMemberSettings settings)
        {
            ValueDiff diff;
            if (TryGetValueDiff(xItem, yItem, settings, out diff))
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

            IRefCounted<DiffBuilder> subDiffBuilder;
            if (DiffBuilder.TryCreate(xItem, yItem, settings, out subDiffBuilder))
            {
                subDiffBuilder.Value.UpdateDiffs(xItem, yItem, settings);
            }

            collectionBuilder.AddLazy(index, subDiffBuilder.Value);
        }

        private static ValueDiff TryCreateValueDiff<T>(T x, T y, IMemberSettings settings)
        {
            Debug.Assert(x != null, "x == null");
            Debug.Assert(y != null, "y == null");
            Debug.Assert(settings != null, "settings == null");

            ValueDiff diff;
            if (TryGetValueDiff(x, y, settings, out diff))
            {
                return diff;
            }

            using (var borrow = DiffBuilder.GetOrCreate(x, y, settings))
            {
                borrow.Value.UpdateDiffs(x, y, settings);
                return borrow.Value.CreateValueDiff();
            }
        }

        private static void TryAddMemberDiffs(
            object x,
            object y,
            IMemberSettings settings,
            DiffBuilder builder)
        {
            foreach (var member in settings.GetMembers(x.GetType()))
            {
                builder.UpdateMemberDiff(x, y, member, settings);
            }
        }

        private static void TryAddCollectionDiffs<TSettings>(
            object x,
            object y,
            TSettings settings,
            DiffBuilder collectionBuilder,
            Action<DiffBuilder, object, object, object, TSettings> itemDiff)
            where TSettings : IMemberSettings
        {
            if (!Is.Enumerable(x, y))
            {
                return;
            }

            IDiffBy comparer;
            if (ListDiffBy.TryGetOrCreate(x, y, out comparer) ||
                ReadonlyListDiffBy.TryGetOrCreate(x, y, out comparer) ||
                ArrayDiffBy.TryGetOrCreate(x, y, out comparer) ||
                DictionaryDiffBy.TryGetOrCreate(x, y, out comparer) ||
                ReadOnlyDictionaryDiffBy.TryGetOrCreate(x, y, out comparer) ||
                SetDiffBy.TryGetOrCreate(x, y, out comparer) ||
                EnumerableDiffBy.TryGetOrCreate(x, y, out comparer))
            {
                comparer.AddDiffs(collectionBuilder, x, y, settings, itemDiff);
                return;
            }

            throw Throw.ShouldNeverGetHereException("All enumarebles must be checked here");
        }
    }
}