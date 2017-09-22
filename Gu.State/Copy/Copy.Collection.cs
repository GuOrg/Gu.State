namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public static partial class Copy
    {
        private static bool IsCopyableCollectionType(Type type)
        {
            return type.IsArray ||
                   typeof(IList).IsAssignableFrom(type) ||
                   type.Implements(typeof(IList<>)) ||
                   typeof(IDictionary).IsAssignableFrom(type) ||
                   type.Implements(typeof(IDictionary<,>)) ||
                   type.Implements(typeof(ISet<>));
        }

        private static void CollectionItems(
            object source,
            object target,
            MemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            if (!Is.Enumerable(source, target))
            {
                return;
            }

            if (settings.ReferenceHandling == ReferenceHandling.Throw &&
                !settings.IsImmutable(source.GetType().GetItemType()))
            {
                throw State.Throw.ShouldNeverGetHereException("Should have been checked for throw before copy");
            }

            if (ArrayCopyer.TryGetOrCreate(source, target, out var copyer) ||
    ListOfTCopyer.TryGetOrCreate(source, target, out copyer) ||
    ListCopyer.TryGetOrCreate(source, target, out copyer) ||
    DictionaryTKeyTValueCopyer.TryGetOrCreate(source, target, out copyer) ||
    DictionaryCopyer.TryGetOrCreate(source, target, out copyer) ||
    SetOfTCopyer.TryGetOrCreate(source, target, out copyer))
            {
                copyer.Copy(source, target, settings, referencePairs);
                return;
            }

            if (source is IEnumerable || target is IEnumerable)
            {
                throw State.Throw.ShouldNeverGetHereException("Should be checked before");
            }
        }
    }
}
