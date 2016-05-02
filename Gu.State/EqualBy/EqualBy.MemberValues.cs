namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Reflection;

    /// <summary>
    /// Defines methods for comparing two instances
    /// </summary>
    public static partial class EqualBy
    {
        internal static bool MemberValues<T>(T x, T y, IMemberSettings settings)
        {
            bool result;
            if (TryGetValueEquals(x, y, settings, out result))
            {
                return result;
            }

            Verify.CanEqualByMemberValues(x, y, settings);
            using (var pairs = settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops
                                   ? ReferencePairCollection.Borrow()
                                   : null)
            {
                return MemberValues(x, y, settings, pairs);
            }
        }

        private static bool MemberValues<T>(
            T x,
            T y,
            IMemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            bool result;
            if (TryGetValueEquals(x, y, settings, out result))
            {
                return result;
            }

            referencePairs?.Add(x, y);

            if (x is IEnumerable)
            {
                if (!EnumerableEquals(x, y, MemberValues, settings, referencePairs))
                {
                    return false;
                }
            }

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
                        return false;
                    }

                    continue;
                }

                var xv = getterAndSetter.GetValue(x);
                var yv = getterAndSetter.GetValue(y);
                if (member.MemberType().IsClass && referencePairs?.Contains(xv, yv) == true)
                {
                    continue;
                }

                if (!MemberValueEquals(xv, yv, member, settings, referencePairs))
                {
                    return false;
                }
            }

            return true;
        }

        // ReSharper disable once UnusedParameter.Local
        private static bool MemberValueEquals(
            object x,
            object y,
            MemberInfo propertyInfo,
            IMemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            bool result;
            if (TryGetValueEquals(x, y, settings, out result))
            {
                return result;
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.References:
                    return ReferenceEquals(x, y);
                case ReferenceHandling.Structural:
                case ReferenceHandling.StructuralWithReferenceLoops:
                    Verify.CanEqualByMemberValues(x, y, settings);
                    return MemberValues(x, y, settings, referencePairs);
                case ReferenceHandling.Throw:
                    throw Throw.ShouldNeverGetHereException();
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(settings.ReferenceHandling),
                        settings.ReferenceHandling,
                        null);
            }
        }

        private static bool TryGetValueEquals<T>(T x, T y, IMemberSettings settings, out bool result)
        {
            if (ReferenceEquals(x, y))
            {
                result = true;
                return true;
            }

            if (x == null || y == null)
            {
                result = false;
                return true;
            }

            if (x.GetType() != y.GetType())
            {
                result = false;
                return true;
            }

            CastingComparer comparer;
            if (settings.TryGetComparer(x.GetType(), out comparer))
            {
                result = comparer.Equals(x, y);
                return true;
            }

            if (settings.IsEquatable(x.GetType()))
            {
                result = Equals(x, y);
                return true;
            }

            result = false;
            return false;
        }
    }
}
