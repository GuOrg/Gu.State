namespace Gu.State
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    internal class DictionaryTKeyTValueCopyer : ICopyer
    {
        public static readonly DictionaryTKeyTValueCopyer Default = new DictionaryTKeyTValueCopyer();

        private DictionaryTKeyTValueCopyer()
        {
        }

        public static bool TryGetOrCreate(object x, object y, out ICopyer comparer)
        {
            if (Is.IDictionaryOfTKeyTValue(x, y))
            {
                comparer = Default;
                return true;
            }

            comparer = null;
            return false;
        }

        public void Copy(
            object source,
            object target,
            MemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            var genericArguments = source.GetType()
                                 .GetInterface("IDictionary`2")
                                 .GetGenericArguments();
            Debug.Assert(genericArguments.Length == 2, "genericArguments.Length != 2");

            var copyMethod = this.GetType()
                                 .GetMethod(nameof(State.Copy), BindingFlags.NonPublic | BindingFlags.Static)
                                 .MakeGenericMethod(genericArguments[0], genericArguments[1]);
            copyMethod.Invoke(null, new[] { source, target, settings, referencePairs });
        }

        internal static void Copy<TKey, TValue>(
            IDictionary<TKey, TValue> source,
            IDictionary<TKey, TValue> target,
            MemberSettings settings,
            ReferencePairCollection referencePairs)
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
                target.TryGetValue(key, out TValue tv);
                var clone = State.Copy.CloneWithoutSync(sv, tv, settings, out bool created, out bool needsSync);
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
