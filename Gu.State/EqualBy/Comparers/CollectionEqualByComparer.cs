namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    internal abstract class CollectionEqualByComparer<TCollection, TItem> : EqualByComparer<TCollection>
    {
        [Obsolete("It has state so Default field can't be used. Adding it here so subclasses do not add it.", error: true)]
        internal static readonly CollectionEqualByComparer<TCollection, TItem> Default;

        protected readonly EqualByComparer ItemComparer;

        protected CollectionEqualByComparer(EqualByComparer itemComparer)
        {
            this.ItemComparer = itemComparer;
        }

        internal override bool CanHaveReferenceLoops => this.ItemComparer.CanHaveReferenceLoops;

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

        internal override bool Equals(object x, object y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
        {
            if (TryGetEitherNullEquals(x, y, out var result))
            {
                return result;
            }

            if (referencePairs != null &&
                ReferencePairStruct.TryCreate(x, y, out var pair))
            {
                if (referencePairs.Add(pair) == false)
                {
                    return true;
                }

                var equals = this.Equals((TCollection)x, (TCollection)y, settings, referencePairs);

                referencePairs.Remove(pair);
                return equals;
            }
            else
            {
                return this.Equals((TCollection)x, (TCollection)y, settings, referencePairs);
            }
        }

        internal override bool TryGetError(MemberSettings settings, out Error error) => TryGetItemError(settings, out error);
    }
}