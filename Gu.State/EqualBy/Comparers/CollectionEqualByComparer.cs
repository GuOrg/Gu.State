namespace Gu.State
{
    internal abstract class CollectionEqualByComparer<TCollection, TItem> : EqualByComparer<TCollection>
    {
        internal static bool TryGetItemError(MemberSettings settings, out Error error)
        {
            var comparer = settings.GetEqualByComparer(typeof(TItem));
            if (comparer is ErrorEqualByComparer errorEqualByComparer)
            {
                error = new TypeErrors(typeof(TCollection), errorEqualByComparer.Error);
                return true;
            }

            if (comparer.TryGetError(settings, out var itemError))
            {
                error = itemError;
                return true;
            }

            error = null;
            return false;
        }

        internal override bool TryGetError(MemberSettings settings, out Error error) => TryGetItemError(settings, out error);
    }
}