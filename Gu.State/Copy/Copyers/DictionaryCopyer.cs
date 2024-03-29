namespace Gu.State
{
    using System.Collections;

    internal sealed class DictionaryCopyer : ICopyer
    {
        internal static readonly DictionaryCopyer Default = new();

        private DictionaryCopyer()
        {
        }

        public void Copy(
            object source,
            object target,
            MemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            Copy((IDictionary)source, (IDictionary)target, settings, referencePairs);
        }

        internal static bool TryGetOrCreate(object x, object y, out ICopyer comparer)
        {
            if (Is.Type<IDictionary>(x, y))
            {
                comparer = Default;
                return true;
            }

            comparer = null;
            return false;
        }

        internal static void Copy(
            IDictionary source,
            IDictionary target,
            MemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            using (var toRemove = ListPool<object>.Borrow())
            {
                foreach (var key in target.Keys)
                {
                    if (!source.Contains(key))
                    {
                        toRemove.Value.Add(key);
                    }
                    else
                    {
                        // Synchronize key?
                    }
                }

                foreach (var key in toRemove.Value)
                {
                    target.Remove(key);
                }
            }

            foreach (var key in source.Keys)
            {
                var sv = source[key];
                var tv = target.ElementAtOrDefault(key);
                var clone = State.Copy.CloneWithoutSync(sv, tv, settings, out var created, out var needsSync);
                if (created)
                {
                    target[key] = clone;
                }

                if (needsSync)
                {
                    State.Copy.Sync(sv, clone, settings, referencePairs);
                }
            }
        }
    }
}
