namespace Gu.State
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    internal sealed class DictionaryTKeyTValueCopyer : ICopyer
    {
        internal static readonly DictionaryTKeyTValueCopyer Default = new();

        private DictionaryTKeyTValueCopyer()
        {
        }

        public void Copy(object source, object target, MemberSettings settings, ReferencePairCollection referencePairs)
        {
            var genericArguments = source.GetType()
                                 .GetInterface("IDictionary`2")
                                 .GetGenericArguments();
            Debug.Assert(genericArguments.Length == 2, "genericArguments.Length != 2");

            var copyMethod = typeof(DictionaryTKeyTValueCopyer)
                                 .GetMethod(nameof(this.Copy), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)
                                 .MakeGenericMethod(genericArguments[0], genericArguments[1]);
            _ = copyMethod.Invoke(null, new[] { source, target, settings, referencePairs });
        }

        internal static bool TryGetOrCreate(object x, object y, out ICopyer comparer)
        {
            if (Is.IDictionaryOfTKeyTValue(x, y))
            {
                comparer = Default;
                return true;
            }

            comparer = null;
            return false;
        }

        internal static void Copy<TKey, TValue>(IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> target, MemberSettings settings, ReferencePairCollection referencePairs)
        {
            using (var toRemove = ListPool<TKey>.Borrow())
            {
                foreach (var key in target.Keys)
                {
                    if (!source.ContainsKey(key))
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
                _ = target.TryGetValue(key, out var tv);
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
