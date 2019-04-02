namespace Gu.State
{
    using System;

    /// <summary>
    /// Defines methods for comparing two instances.
    /// </summary>
    public static partial class EqualBy
    {
        internal static bool MemberValues<T>(T x, T y, MemberSettings settings)
        {
            using (var referencePairs = GetReferencePairs())
            {
                return MemberValues(x, y, settings, referencePairs?.Value);
            }

            IBorrowed<ReferencePairCollection> GetReferencePairs()
            {
                if (settings.ReferenceHandling == ReferenceHandling.Structural)
                {
                    return ReferencePairCollection.Borrow();
                }

                return null;
            }
        }

        internal static bool MemberValues<T>(T x, T y, MemberSettings settings, ReferencePairCollection referencePairs)
        {
            Verify.CanEqualByMemberValues(x, y, settings, typeof(EqualBy).Name, settings.EqualByMethodName());
            return settings.GetEqualByComparer(x?.GetType() ?? y?.GetType() ?? typeof(T), checkReferenceHandling: false)
                           .Equals(x, y, settings, referencePairs);
        }

        [Obsolete("This can probably be removed.")]
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
    }
}
