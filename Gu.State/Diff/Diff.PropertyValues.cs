namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    public partial class DiffBy
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

            if (settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops)
            {
                var referencePairs = new ReferencePairCollection();
                return PropertiesValuesDiff(x, y, settings, referencePairs);
            }

            return PropertiesValuesDiff(x, y, settings, null);
        }

        private static Diff PropertiesValuesDiff<T>(
            T x,
            T y,
            PropertiesSettings settings,
            ReferencePairCollection referencePairs)
        {
            referencePairs?.Add(x, y);
            if (ReferenceEquals(x, y))
            {
                return Diff.Empty;
            }

            if (x == null || y == null)
            {
                return new ValueDiff(x, y);
            }

            if (x.GetType() != y.GetType())
            {
                return new ValueDiff(x, y);
            }

            if (x.GetType().IsEquatable())
            {
                return Equals(x, y)
                           ? Diff.Empty
                           : new ValueDiff(x, y);
            }

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
                if (propDiff.IsEmpty)
                {
                    continue;
                }

                if (diffs == null)
                {
                    diffs = new List<Diff>();
                }

                diffs.Add(propDiff);
            }

            return diffs == null ? Diff.Empty : new Diff(diffs);
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
            if (ReferenceEquals(xValue, yValue))
            {
                return Diff.Empty;
            }

            if (xValue == null || yValue == null)
            {
                return new PropertyDiff(propertyInfo, xValue, yValue);
            }

            if (xValue.GetType().IsEquatable())
            {
                return Equals(xValue, yValue) ? Diff.Empty : new PropertyDiff(propertyInfo, xValue, yValue);
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.References:
                    return ReferenceEquals(xValue, yValue) ? Diff.Empty : new PropertyDiff(propertyInfo, xValue, yValue);
                case ReferenceHandling.Structural:
                case ReferenceHandling.StructuralWithReferenceLoops:
                    EqualBy.Verify.CanEqualByPropertyValues(xValue, yValue, settings);
                    var diff = PropertiesValuesDiff(xValue, yValue, settings, referencePairs);
                    return diff.IsEmpty
                               ? diff
                               : new PropertyDiff(propertyInfo, xValue, yValue, diff);

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
