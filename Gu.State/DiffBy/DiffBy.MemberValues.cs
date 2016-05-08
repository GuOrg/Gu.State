namespace Gu.State
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    public static partial class DiffBy
    {
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

            using (var borrow = DiffBuilder.Create(x, y, settings))
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
            EqualBy.Verify.CanEqualByMemberValues(x, y, settings, typeof(DiffBy).Name, settings.DiffMethodName());
            TryAddItemDiffs(x, y, settings, builder, TryAddItemDiff);
            TryAddMemberDiffs(x, y, settings, builder);
        }

        private static void TryAddMemberDiffs(
            object x,
            object y,
            IMemberSettings settings,
            DiffBuilder builder)
        {
            foreach (var member in settings.GetMembers(x.GetType()))
            {
                TryAddMemberDiff(x, y, member, settings, builder);
            }
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
                    IRefCounted<DiffBuilder> subDiffBuilder;
                    if (DiffBuilder.TryCreate(xValue, yValue, settings, out subDiffBuilder))
                    {
                        TryAddDiffs(xValue, yValue, settings, subDiffBuilder.Value);
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

        private static void TryAddItemDiffs<TSettings>(
            object x,
            object y,
            TSettings settings,
            DiffBuilder collectionBuilder,
            Action<object, object, object, TSettings, DiffBuilder> itemDiff)
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
                comparer.AddDiffs(x, y, settings, collectionBuilder, itemDiff);
                return;
            }

            throw Throw.ShouldNeverGetHereException("All enumarebles must be checked here");
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

            IRefCounted<DiffBuilder> subDiffBuilder;
            if (DiffBuilder.TryCreate(xItem, yItem, settings, out subDiffBuilder))
            {
                TryAddDiffs(xItem, yItem, settings, subDiffBuilder.Value);
            }

            collectionBuilder.AddLazy(index, subDiffBuilder.Value);
        }
    }
}