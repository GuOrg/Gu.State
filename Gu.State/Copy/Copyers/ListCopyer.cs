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
            Func<object, object, TSettings, ReferencePairCollection, object> copyItem,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            Copy((IList)source, (IList)target, copyItem, settings, referencePairs);
        }

        private static void Copy<TSettings>(
            IList source,
            IList target,
            Func<object, object, TSettings, ReferencePairCollection, object> copyItem,
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
                var copy = State.Copy.Item(sv, tv, copyItem, settings, referencePairs, isImmutable);
                if (i < target.Count)
                {
                    target[i] = copy;
                }
                else
                {
                    target.Add(copy);
                }
            }

            target.TrimLengthTo(source);
        }
    }
}