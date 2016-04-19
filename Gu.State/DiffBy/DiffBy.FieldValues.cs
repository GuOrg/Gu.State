namespace Gu.State
{
    using System;
    using System.Collections.Generic;
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
            return FieldValuesCore(x, y, settings) ?? new EmptyDiff(x, y);
        }

        private static Diff FieldValuesCore<T>(T x, T y, FieldsSettings settings)
        {
            ValueDiff diff;
            if (TryGetValueDiff(x, y, settings, out diff))
            {
                return diff;
            }

            EqualBy.Verify.CanEqualByFieldValues(x, y, settings, typeof(DiffBy).Name, nameof(FieldValues));
            IReadOnlyList<SubDiff> diffs;
            using (var pairs = settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops
                                   ? ReferencePairCollection.Create()
                                   : null)
            {
                diffs = SubDiffs(x, y, settings, pairs);
            }

            return diffs == null
                       ? null
                       : new ValueDiff(x, y, diffs);
        }

        private static IReadOnlyList<SubDiff> SubDiffs<T>(
            T x,
            T y,
            FieldsSettings settings,
            ReferencePairCollection referencePairs)
        {
            referencePairs?.Add(x, y);
            var diffs = Enumerable.Diffs(x, y, settings, referencePairs, ItemFieldsDiff);

            var fieldInfos = x.GetType().GetFields(settings.BindingFlags);
            foreach (var fieldInfo in fieldInfos)
            {
                if (settings.IsIgnoringField(fieldInfo))
                {
                    continue;
                }

                var xv = fieldInfo.GetValue(x);
                var yv = fieldInfo.GetValue(y);
                if (referencePairs?.Contains(xv, yv) == true)
                {
                    continue;
                }

                var fieldValueDiff = FieldValueDiff(xv, yv, fieldInfo, settings, referencePairs);
                if (fieldValueDiff == null)
                {
                    continue;
                }

                if (diffs == null)
                {
                    diffs = new List<SubDiff>();
                }

                diffs.Add(fieldValueDiff);
            }

            return diffs;
        }

        private static ValueDiff ItemFieldsDiff(
            object x,
            object y,
            FieldsSettings settings,
            ReferencePairCollection referencePairs)
        {
            ValueDiff diff;
            if (TryGetValueDiff(x, y, settings, out diff))
            {
                return diff;
            }

            if (settings.ReferenceHandling == ReferenceHandling.References)
            {
                return ReferenceEquals(x, y)
                           ? null
                           : new ValueDiff(x, y);
            }

            EqualBy.Verify.CanEqualByFieldValues(x, y, settings, typeof(DiffBy).Name, nameof(FieldValues));
            var diffs = SubDiffs(x, y, settings, referencePairs);
            return diffs == null
                       ? null
                       : new ValueDiff(x, y, diffs);
        }

        private static SubDiff FieldValueDiff(
            object xValue,
            object yValue,
            FieldInfo fieldInfo,
            FieldsSettings settings,
            ReferencePairCollection referencePairs)
        {
            ValueDiff diff;
            if (TryGetValueDiff(xValue, yValue, settings, out diff))
            {
                return diff == null
                           ? null
                           : new FieldDiff(fieldInfo, diff);
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.References:
                    return ReferenceEquals(xValue, yValue) ? null : new FieldDiff(fieldInfo, xValue, yValue);
                case ReferenceHandling.Structural:
                case ReferenceHandling.StructuralWithReferenceLoops:
                    EqualBy.Verify.CanEqualByFieldValues(xValue, yValue, settings);
                    var diffs = SubDiffs(xValue, yValue, settings, referencePairs);
                    return diffs == null
                               ? null
                               : new FieldDiff(fieldInfo, new ValueDiff(xValue, yValue, diffs));

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
