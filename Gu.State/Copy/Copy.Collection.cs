namespace Gu.State
{
    using System;
    using System.Collections;

    public static partial class Copy
    {
        internal static T Item<T, TSettings>(
            T sourceItem,
            T targetItem,
            Func<object, object, TSettings, ReferencePairCollection, object> copyItem,
            TSettings settings,
            ReferencePairCollection referencePairs,
            bool isImmutable)
            where TSettings : class, IMemberSettings
        {
            if (sourceItem == null || settings.ReferenceHandling == ReferenceHandling.References || isImmutable)
            {
                return sourceItem;
            }

            T copy;
            if (MemberValues.TryCustomCopy(sourceItem, targetItem, settings, out copy))
            {
                return copy;
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.References:
                    return sourceItem;
                case ReferenceHandling.Structural:
                    if (targetItem == null)
                    {
                        targetItem = (T)State.Copy.CreateInstance(sourceItem, null, settings);
                    }

                    copyItem(sourceItem, targetItem, settings, referencePairs);
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

        private static void CopyCollectionItems<TSettings>(
            object source,
            object target,
            Func<object, object, TSettings, ReferencePairCollection, object> copyItem,
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
                copyer.Copy(source, target, copyItem, settings, referencePairs);
                return;
            }

            if (source is IEnumerable || target is IEnumerable)
            {
                throw State.Throw.ShouldNeverGetHereException("Should be checked before");
            }
        }
    }
}
