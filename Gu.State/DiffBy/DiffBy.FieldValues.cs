namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    public static partial class DiffBy
    {
        /// <summary>
        /// Compares x and y for equality using field values.
        /// If a type implements IList the items of the list are compared
        /// </summary>
        /// <typeparam name="T">The type to compare</typeparam>
        /// <param name="x">The first instance</param>
        /// <param name="y">The second instance</param>
        /// <param name="referenceHandling">
        /// If Structural is used a deep equals is performed.
        /// Default value is Throw
        /// </param>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <returns>Diff.Empty if <paramref name="x"/> and <paramref name="y"/> are equal</returns>
        public static Diff FieldValues<T>(
            T x,
            T y,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw,
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags)
        {
            var settings = FieldsSettings.GetOrCreate(bindingFlags, referenceHandling);
            return FieldValues(x, y, settings);
        }

        /// <summary>
        /// Compares x and y for equality using field values and returns the difference.
        /// If a type implements IList the items of the list are compared
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="x"/> and <paramref name="y"/></typeparam>
        /// <param name="x">The first instance</param>
        /// <param name="y">The second instance</param>
        /// <param name="settings">Specifies how equality is performed.</param>
        /// <returns>Diff.Empty if <paramref name="x"/> and <paramref name="y"/> are equal</returns>
        public static Diff FieldValues<T>(T x, T y, FieldsSettings settings)
        {
            Ensure.NotNull(x, nameof(x));
            Ensure.NotNull(y, nameof(y));
            Ensure.NotNull(settings, nameof(settings));
            EqualBy.Verify.CanEqualByFieldValues(x, y, settings, typeof(DiffBy).Name, nameof(FieldValues));
            return FieldsValuesDiffs.Get(x, y, settings) ?? new EmptyDiff(x, y);
        }

        private static class FieldsValuesDiffs
        {
            internal static ValueDiff Get<T>(T x, T y, FieldsSettings settings)
            {
                Debug.Assert(x != null, "x == null");
                Debug.Assert(y != null, "y == null");
                Debug.Assert(settings != null, "settings == null");

                ValueDiff diff;
                if (TryGetValueDiff(x, y, settings, out diff))
                {
                    return diff;
                }

                EqualBy.Verify.CanEqualByFieldValues(x, y, settings, typeof(DiffBy).Name, nameof(FieldValues));
                var builder = new DiffBuilderRoot(x, y, settings.ReferenceHandling);
                AddSubDiffs(x, y, settings, builder);
                return builder.CreateValueDiff();
            }

            private static void AddSubDiffs<T>(
                T x,
                T y,
                FieldsSettings settings,
                DiffBuilder builder)
            {
                Enumerable.AddItemDiffs(x, y, settings, builder, ItemFieldsDiff);
                var fieldInfos = x.GetType().GetFields(settings.BindingFlags);
                foreach (var fieldInfo in fieldInfos)
                {
                    if (settings.IsIgnoringField(fieldInfo))
                    {
                        continue;
                    }

                    var getterAndSetter = settings.GetOrCreateGetterAndSetter(fieldInfo);
                    if (settings.IsEquatable(getterAndSetter.ValueType))
                    {
                        if (!getterAndSetter.ValueEquals(x, y))
                        {
                            builder.Add(new FieldDiff(fieldInfo, getterAndSetter.GetValue(x), getterAndSetter.GetValue(y)));
                        }

                        continue;
                    }

                    var xv = getterAndSetter.GetValue(x);
                    var yv = getterAndSetter.GetValue(y);
                    FieldValueDiff(xv, yv, fieldInfo, settings, builder);
                }
            }

            private static void ItemFieldsDiff(
                object xItem,
                object yItem,
                object index,
                FieldsSettings settings,
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

                EqualBy.Verify.CanEqualByFieldValues(xItem, yItem, settings, typeof(DiffBy).Name, nameof(PropertyValues));
                DiffBuilder subDiffBuilder;
                if (collectionBuilder.TryAdd(xItem, yItem, out subDiffBuilder))
                {
                    AddSubDiffs(xItem, yItem, settings, subDiffBuilder);
                }

                collectionBuilder.AddLazy(index, subDiffBuilder);
            }

            private static void FieldValueDiff(
                object xValue,
                object yValue,
                FieldInfo fieldInfo,
                FieldsSettings settings,
                DiffBuilder builder)
            {
                ValueDiff diff;
                if (TryGetValueDiff(xValue, yValue, settings, out diff))
                {
                    if (diff != null)
                    {
                        builder.Add(new FieldDiff(fieldInfo, diff));
                    }

                    return;
                }

                switch (settings.ReferenceHandling)
                {
                    case ReferenceHandling.References:
                        if (!ReferenceEquals(xValue, yValue))
                        {
                            builder.Add(new FieldDiff(fieldInfo, new ValueDiff(xValue, yValue)));
                        }

                        return;
                    case ReferenceHandling.Structural:
                    case ReferenceHandling.StructuralWithReferenceLoops:
                        EqualBy.Verify.CanEqualByFieldValues(xValue, yValue, settings, typeof(DiffBy).Name, nameof(PropertyValues));
                        DiffBuilder subDiffBuilder;
                        if (builder.TryAdd(xValue, yValue, out subDiffBuilder))
                        {
                            AddSubDiffs(xValue, yValue, settings, subDiffBuilder);
                        }

                        builder.AddLazy(fieldInfo, subDiffBuilder);
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
