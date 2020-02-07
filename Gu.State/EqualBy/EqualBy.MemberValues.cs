namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines methods for comparing two instances.
    /// </summary>
    public static partial class EqualBy
    {
        internal static bool MemberValues<T>(T x, T y, MemberSettings settings)
        {
            var equalByComparer = settings.GetRootEqualByComparer(x?.GetType() ?? y?.GetType() ?? typeof(T));
            using var referencePairs = equalByComparer.CanHaveReferenceLoops ? HashSetPool<ReferencePairStruct>.Borrow(EqualityComparer<ReferencePairStruct>.Default) : null;
            try
            {
                return equalByComparer.Equals(x, y, settings, referencePairs?.Value);
            }
            catch (InvalidOperationException e) when (e.Message == nameof(Throw.CompareWhenError))
            {
                ThrowIfHasErrors(settings.GetRootEqualByComparer(x?.GetType() ?? y?.GetType() ?? typeof(T)), settings, typeof(EqualBy).Name, settings is FieldsSettings ? nameof(FieldValues) : nameof(PropertyValues));
                throw;
            }
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
