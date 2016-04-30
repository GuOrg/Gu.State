namespace Gu.State
{
    using System;
    using System.Collections;

    public class ListCopyer : ICopyer
    {
        public static readonly ListCopyer Default = new ListCopyer();

        private ListCopyer()
        {
        }

        public static bool TryGetOrCreate(object x, object y, out ICopyer comparer)
        {
            if (x is IList && y is IList)
            {
                comparer = Default;
                return true;
            }

            comparer = null;
            return false;
        }

        public void Copy<TSettings>(
            object source,
            object target,
            Action<object, object, TSettings, ReferencePairCollection> syncItem,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            Copy((IList)source, (IList)target, syncItem, settings, referencePairs);
        }

        private static void Copy<TSettings>(
            IList source,
            IList target,
            Action<object, object, TSettings, ReferencePairCollection> syncItem,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            if ((source.IsFixedSize || target.IsFixedSize) && source.Count != target.Count)
            {
                throw State.Copy.Throw.CannotCopyFixesSizeCollections(source, target, settings);
            }

            var isImmutable = settings.IsImmutable(source.GetType().GetItemType());
            for (var i = 0; i < source.Count; i++)
            {
                var sv = source[i];
                var tv = target.ElementAtOrDefault(i);
                var copyItem = State.Copy.Item(sv, tv, syncItem, settings, referencePairs, isImmutable);
                if (i < target.Count)
                {
                    target[i] = copyItem;
                }
                else
                {
                    target.Add(copyItem);
                }
            }

            for (var i = target.Count - 1; i >= source.Count; i--)
            {
                target.RemoveAt(i);
            }
        }
    }
}