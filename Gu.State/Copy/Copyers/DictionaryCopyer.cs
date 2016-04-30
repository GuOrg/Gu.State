namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Linq;

    public class DictionaryCopyer : ICopyer
    {
        public static readonly DictionaryCopyer Default = new DictionaryCopyer();

        private DictionaryCopyer()
        {
        }

        public static bool TryGetOrCreate(object x, object y, out ICopyer comparer)
        {
            if (x is IDictionary && y is IDictionary)
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
            Copy((IDictionary)source, (IDictionary)target, syncItem, settings, referencePairs);
        }

        internal static void Copy<TSettings>(
            IDictionary source,
            IDictionary target,
            Action<object, object, TSettings, ReferencePairCollection> syncItem,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            foreach (var key in source.Keys)
            {
                var sv = source[key];
                var tv = target.ElementAtOrDefault(key);
                var copyItem = State.Copy.Item(sv, tv, syncItem, settings, referencePairs, settings.IsImmutable(sv.GetType()));
                target[key] = copyItem;
            }

            var toRemove = target.Keys.Cast<object>()
                                     .Where(x => !source.Contains(x))
                                     .ToList();
            foreach (var key in toRemove)
            {
                target.Remove(key);
            }
        }
    }
}
