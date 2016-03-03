namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Reflection;

    public static partial class EqualBy
    {
        /// <summary>
        /// Compares x and y for equality using field values.
        /// If a type implements IList the items of the list are compared.
        /// Event fields are excluded.
        /// For performance the overload with settings should be used and the settings should be cached.
        /// </summary>
        /// <typeparam name="T">The type to get ignore fields for settings for</typeparam>
        /// <param name="x">The first instance</param>
        /// <param name="y">The second instance</param>
        /// <param name="bindingFlags">The binding flags to use when getting fields</param>
        /// <param name="referenceHandling">
        /// If Structural is used a deep equals is performed.
        /// Default value is Throw
        /// </param>
        /// <returns>True if <paramref name="x"/> and <paramref name="y"/> are equal</returns>
        public static bool FieldValues<T>(T x, T y, BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags, ReferenceHandling referenceHandling = ReferenceHandling.Throw)
        {
            var settings = FieldsSettings.GetOrCreate(bindingFlags, referenceHandling);
            return FieldValues(x, y, settings);
        }

        /// <summary>
        /// Compares x and y for equality using field values.
        /// If a type implements IList the items of the list are compared.
        /// Event fields are excluded.
        /// For performance the overload with settings should be used and the settings should be cached.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="x"/> and <paramref name="y"/></typeparam>
        /// <param name="x">The first instance</param>
        /// <param name="y">The second instance</param>
        /// <param name="settings">Specifies how equality is performed.</param>
        /// <returns>True if <paramref name="x"/> and <paramref name="y"/> are equal</returns>
        public static bool FieldValues<T>(T x, T y, FieldsSettings settings)
        {
            Verify.CanEqualByFieldValues(x, y, settings);
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            if (x.GetType().IsEquatable())
            {
                return Equals(x, y);
            }

            if (settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops)
            {
                var referencePairs = new ReferencePairCollection();
                return FieldsValuesEquals(x, y, settings, referencePairs);
            }

            return FieldsValuesEquals(x, y, settings, null);
        }

        private static bool FieldsValuesEquals(object x, object y, FieldsSettings settings, ReferencePairCollection referencePairs)
        {
            referencePairs?.Add(x, y);

            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            if (x is IEnumerable)
            {
                if (!EnumerableEquals(x, y, FieldItemEquals, settings, referencePairs))
                {
                    return false;
                }
            }

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

                if (!FieldValueEquals(xv, yv, fieldInfo, settings, referencePairs))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool FieldItemEquals(object x, object y, FieldsSettings settings, ReferencePairCollection referencePairs)
        {
            return FieldValueEquals(x, y, null, settings, referencePairs);
        }

        private static bool FieldValueEquals(object x, object y, FieldInfo fieldInfo, FieldsSettings settings, ReferencePairCollection referencePairs)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x.GetType().IsEquatable())
            {
                return Equals(x, y);
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.References:
                    return ReferenceEquals(x, y);
                case ReferenceHandling.Structural:
                case ReferenceHandling.StructuralWithReferenceLoops:
                    Verify.CanEqualByFieldValues(x, y, settings);
                    return FieldsValuesEquals(x, y, settings, referencePairs);
                case ReferenceHandling.Throw:
                    throw ChangeTracking.Throw.ThrowThereIsABugInTheLibrary("Should never get here");
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.ReferenceHandling), settings.ReferenceHandling, null);
            }
        }
    }
}