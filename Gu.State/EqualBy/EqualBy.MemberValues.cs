﻿namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Reflection;

    /// <summary>
    /// Defines methods for comparing two instances
    /// </summary>
    public static partial class EqualBy
    {
        internal static bool MemberValues<T>(T x, T y, MemberSettings settings)
        {
            if (TryGetValueEquals(x, y, settings, out var result))
            {
                return result;
            }

            Verify.CanEqualByMemberValues(x, y, settings, typeof(EqualBy).Name, settings.EqualByMethodName());
            using (var borrowed = settings.ReferenceHandling == ReferenceHandling.Structural
                                   ? ReferencePairCollection.Borrow()
                                   : null)
            {
                return MemberValues(x, y, settings, borrowed?.Value);
            }
        }

        internal static bool MemberValues<T>(
            T x,
            T y,
            MemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            if (TryGetValueEquals(x, y, settings, out var result))
            {
                return result;
            }

            referencePairs?.Add(x, y);

            if (x is IEnumerable)
            {
                if (!EnumerableEquals(x, y, settings, referencePairs))
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
                if (getterAndSetter.TryGetValueEquals(x, y, settings, out var equal, out var xv, out var yv))
                {
                    if (!equal)
                    {
                        return false;
                    }

                    continue;
                }

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

        internal static bool TryGetValueEquals<T>(T x, T y, MemberSettings settings, out bool result)
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

            if (settings.TryGetComparer(x.GetType(), out var comparer))
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

        // ReSharper disable once UnusedParameter.Local
        private static bool MemberValueEquals(
            object x,
            object y,
            MemberInfo propertyInfo,
            MemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            if (TryGetValueEquals(x, y, settings, out var result))
            {
                return result;
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.References:
                    return ReferenceEquals(x, y);
                case ReferenceHandling.Structural:
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
    }
}
