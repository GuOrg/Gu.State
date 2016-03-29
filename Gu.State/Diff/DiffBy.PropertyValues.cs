namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    public static partial class DiffBy
    {
        /// <summary>
        /// Compares x and y for equality using property values.
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
        public static Diff PropertyValues<T>(
            T x,
            T y,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags)
        {
            var settings = PropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
            return PropertyValues(x, y, settings);
        }

        /// <summary>
        /// Compares x and y for equality using property values and returns the difference.
        /// If a type implements IList the items of the list are compared
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="x"/> and <paramref name="y"/></typeparam>
        /// <param name="x">The first instance</param>
        /// <param name="y">The second instance</param>
        /// <param name="settings">Specifies how equality is performed.</param>
        /// <returns>Diff.Empty if <paramref name="x"/> and <paramref name="y"/> are equal</returns>
        public static Diff PropertyValues<T>(T x, T y, PropertiesSettings settings)
        {
            EqualBy.Verify.CanEqualByPropertyValues(x, y, settings);

            ValueDiff diff;
            if (TryGetValueDiff(x, y, out diff))
            {
                return diff;
            }

            var pairs = settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops
                            ? new ReferencePairCollection()
                            : null;
            var diffs = SubDiffs(x, y, settings, pairs);
            return diffs == null
                       ? null
                       : new ValueDiff(x, y, diffs);
        }

        private static IReadOnlyCollection<Diff> SubDiffs<T>(
            T x,
            T y,
            PropertiesSettings settings,
            ReferencePairCollection referencePairs)
        {
            referencePairs?.Add(x, y);
            if (x is IEnumerable)
            {
                throw new NotImplementedException();
                //if (!EnumerableDiff(x, y, ItemPropertiesDiff, settings, referencePairs))
                //{
                //    return false;
                //}
            }

            List<Diff> diffs = null;
            var propertyInfos = x.GetType().GetProperties(settings.BindingFlags);
            foreach (var propertyInfo in propertyInfos)
            {
                if (settings.IsIgnoringProperty(propertyInfo))
                {
                    continue;
                }

                var xv = propertyInfo.GetValue(x);
                var yv = propertyInfo.GetValue(y);
                if (referencePairs?.Contains(xv, yv) == true)
                {
                    continue;
                }

                var propDiff = PropertyValueDiff(xv, yv, propertyInfo, settings, referencePairs);
                if (propDiff == null)
                {
                    continue;
                }

                if (diffs == null)
                {
                    diffs = new List<Diff>();
                }

                diffs.Add(propDiff);
            }

            return diffs;
        }

        private static Diff ItemPropertiesDiff(
            object x,
            object y,
            PropertiesSettings settings,
            ReferencePairCollection referencePairs)
        {
            return PropertyValueDiff(x, y, null, settings, referencePairs);
        }

        private static Diff PropertyValueDiff(
            object xValue,
            object yValue,
            PropertyInfo propertyInfo,
            PropertiesSettings settings,
            ReferencePairCollection referencePairs)
        {
            ValueDiff diff;
            if (TryGetValueDiff(xValue, yValue, out diff))
            {
                return diff == null
                           ? null
                           : new PropertyDiff(propertyInfo, diff);
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.References:
                    return ReferenceEquals(xValue, yValue) ? null : new PropertyDiff(propertyInfo, xValue, yValue);
                case ReferenceHandling.Structural:
                case ReferenceHandling.StructuralWithReferenceLoops:
                    EqualBy.Verify.CanEqualByPropertyValues(xValue, yValue, settings);
                    var diffs = SubDiffs(xValue, yValue, settings, referencePairs);
                    return diff == null
                               ? null
                               : new PropertyDiff(propertyInfo, new ValueDiff(xValue, yValue, diffs));

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
