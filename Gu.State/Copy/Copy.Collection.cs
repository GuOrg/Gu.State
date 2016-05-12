namespace Gu.State
{
    using System;
    using System.Collections;

    public static partial class Copy
    {
        private static void CollectionItems<TSettings>(
            object source,
            object target,
            TSettings settings,
            ReferencePairCollection referencePairs)
             where TSettings : class, IMemberSettings
        {
            if (!Is.Enumerable(source, target))
            {
                return;
            }

            if (!settings.IsEquatable(source.GetType().GetItemType()) && settings.ReferenceHandling == ReferenceHandling.Throw)
            {
                throw State.Throw.ShouldNeverGetHereException("Should have been checked for throw before copy");
            }

            ICopyer copyer;
            if (ArrayCopyer.TryGetOrCreate(source, target, out copyer) ||
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
