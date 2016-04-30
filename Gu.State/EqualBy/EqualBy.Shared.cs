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
            Verify.CanEqualByMemberValues(x, y, settings);
            using (var pairs = settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops
                                   ? ReferencePairCollection.Create()
                                   : null)
            {
                return MemberValuesEquals(x, y, settings, pairs);
            }
        }

        private static bool MemberValuesEquals<T>(
            T x,
            T y,
            IMemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            referencePairs?.Add(x, y);
            if (ReferenceEquals(x, y))
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

            if (settings.IsEquatable(x.GetType()))
            {
                return Equals(x, y);
            }

            if (x is IEnumerable)
            {
                if (!EnumerableEquals(x, y, MemberValuesEquals, settings, referencePairs))
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
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (settings.IsEquatable(x.GetType()))
            {
                return Equals(x, y);
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.References:
                    return ReferenceEquals(x, y);
                case ReferenceHandling.Structural:
                case ReferenceHandling.StructuralWithReferenceLoops:
                    Verify.CanEqualByMemberValues(x, y, settings);
                    return MemberValuesEquals(x, y, settings, referencePairs);
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
