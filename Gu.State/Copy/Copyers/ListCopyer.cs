namespace Gu.State
{
    using System.Collections;

    internal sealed class ListCopyer : ICopyer
    {
        internal static readonly ListCopyer Default = new ListCopyer();

        private ListCopyer()
        {
        }

        public void Copy(
            object source,
            object target,
            MemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            Copy((IList)source, (IList)target, settings, referencePairs);
        }

        internal static bool TryGetOrCreate(object x, object y, out ICopyer comparer)
        {
            if (x is IList && y is IList)
            {
                comparer = Default;
                return true;
            }

            comparer = null;
            return false;
        }

        private static void Copy(
            IList source,
            IList target,
            MemberSettings settings,
            ReferencePairCollection referencePairs)
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
                var clone = State.Copy.CloneWithoutSync(sv, tv, settings, out var created, out var needsSync);
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
