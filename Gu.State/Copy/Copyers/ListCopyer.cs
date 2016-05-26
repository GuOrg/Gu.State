namespace Gu.State
{
    using System.Collections;

    internal class ListCopyer : ICopyer
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
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            Copy((IList)source, (IList)target, settings, referencePairs);
        }

        private static void Copy<TSettings>(
            IList source,
            IList target,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            if ((source.IsFixedSize || target.IsFixedSize) && source.Count != target.Count)
            {
                throw State.Copy.Throw.CannotCopyFixesSizeCollections(source, target, settings);
            }

            var copyValues = State.Copy.IsCopyValue(
                        source.GetType().GetItemType(),
                        settings);
            for (var i = 0; i < source.Count; i++)
            {
                if (copyValues)
                {
                    target.SetElementAt(i, source[i]);
                    continue;
                }

                var sv = source[i];
                var tv = target.ElementAtOrDefault(i);
                bool created;
                bool needsSync;
                var clone = State.Copy.CloneWithoutSync(sv, tv, settings, out created, out needsSync);
                if (created)
                {
                    target.SetElementAt(i, clone);
                }

                if (needsSync)
                {
                    State.Copy.Sync(sv, clone, settings, referencePairs);
                }
            }

            target.TrimLengthTo(source);
        }
    }
}