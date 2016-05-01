namespace Gu.State
{
    using System;
    using System.Collections;

    public static partial class Copy
    {
        internal static T Item<T, TSettings>(
            T sourceItem,
            T targetItem,
            Action<object, object, TSettings, ReferencePairCollection> syncItem,
            TSettings settings,
            ReferencePairCollection referencePairs,
            bool isImmutable)
            where TSettings : class, IMemberSettings
        {
            if (sourceItem == null || settings.ReferenceHandling == ReferenceHandling.References || isImmutable)
            {
                return sourceItem;
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.References:
                    return sourceItem;
                case ReferenceHandling.Structural:
                case ReferenceHandling.StructuralWithReferenceLoops:
                    if (targetItem == null)
                    {
                        targetItem = (T)State.Copy.CreateInstance(sourceItem, null, settings);
                    }

                    syncItem(sourceItem, targetItem, settings, referencePairs);
                    return targetItem;
                case ReferenceHandling.Throw:
                    throw State.Throw.ShouldNeverGetHereException();
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(settings.ReferenceHandling),
                        settings.ReferenceHandling,
                        null);
            }
        }

        private static void CopyCollectionItems<T>(
            object source,
            object target, 
            Action<object, object, T, ReferencePairCollection> syncItem,
            T settings, 
            ReferencePairCollection referencePairs)
             where T : class, IMemberSettings
        {
            if (!Is.Enumerable(source, target))
            {
                return;
            }

            if (!settings.IsEquatable(source.GetType().GetItemType()) && settings.ReferenceHandling == ReferenceHandling.Throw)
            {
                throw Gu.State.Throw.ShouldNeverGetHereException("Should have been checked for throw before copy");
            }

            ICopyer copyer;
            if (ArrayCopyer.TryGetOrCreate(source, target, out copyer) ||
                ListOfTCopyer.TryGetOrCreate(source, target, out copyer) ||
                ListCopyer.TryGetOrCreate(source, target, out copyer) ||
                DictionaryTKeyTValueCopyer.TryGetOrCreate(source, target, out copyer) ||
                DictionaryCopyer.TryGetOrCreate(source, target, out copyer) ||
                SetOfTCopyer.TryGetOrCreate(source, target, out copyer))
            {
                copyer.Copy(source, target, syncItem, settings, referencePairs);
                return;
            }

            if (source is IEnumerable || target is IEnumerable)
            {
                throw State.Throw.ShouldNeverGetHereException("Should be checked before");
            }
        }
    }
}
